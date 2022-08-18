using es.dmoreno.utils.security;
using pga.core.DTOsBox;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Text;
using System.Threading.Tasks;

namespace pga.core
{
    public class BoxSubject
    {
        private Box Box { get; }

        internal BoxSubject(Box b)
        {
            this.Box = b;
        }

        internal async Task<DTOBoxSubjectRoot> GetRoot()
        {
            var db_subjectroot = await this.Box.DBLogic.ProxyStatement<DTOBoxSubjectRoot>();
            var root = await db_subjectroot.FirstIfExistsAsync<DTOBoxSubjectRoot>();
            if (root == null)
            {
                //No existe usuario root, se crea uno por defecto
                var db_subject = await this.Box.DBLogic.ProxyStatement<DTOBoxSubject>();
                await db_subject.insertAsync(new DTOBoxSubject { 
                    Name = "Root",
                    UUID = Guid.NewGuid().ToString()
                });
                root = new DTOBoxSubjectRoot { 
                    RefSubject = db_subject.lastID,
                    UserMD5 = MD5Utils.GetHash("admin"),
                    PasswordMD5 = MD5Utils.GetHash("password")
                };
                await db_subjectroot.insertAsync(root);
            }
            return root;
        }

        internal async Task<bool> IsRoot(DTOBoxSubject s)
        {
            var actual_root = await this.GetRoot();
            return s.ID == actual_root.RefSubject;
        }
    }
}
