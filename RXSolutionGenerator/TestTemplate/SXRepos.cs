﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// DO NOT MODIFY THIS FILE! It is regenerated by the designer.
// All your modifications will be lost!
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace TestTemplate
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    /// The class representing the SXRepos element repository.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Ranorex", "8.2")]
    [RepositoryFolder("f54ab49e-2074-43a3-bc83-bd6f67c47df2")]
    public partial class SXRepos : RepoGenBaseFolder
    {
        static SXRepos instance = new SXRepos();
        SXReposFolders.FrmStartmenyAppFolder _frmstartmeny;
        SXReposFolders.FrmPassordAppFolder _frmpassord;
        SXReposFolders.FrmConfirmAppFolder _frmconfirm;

        /// <summary>
        /// Gets the singleton class instance representing the SXRepos element repository.
        /// </summary>
        [RepositoryFolder("f54ab49e-2074-43a3-bc83-bd6f67c47df2")]
        public static SXRepos Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Repository class constructor.
        /// </summary>
        public SXRepos() 
            : base("SXRepos", "/", null, 0, false, "f54ab49e-2074-43a3-bc83-bd6f67c47df2", ".\\RepositoryImages\\SXReposf54ab49e.rximgres")
        {
            _frmstartmeny = new SXReposFolders.FrmStartmenyAppFolder(this);
            _frmpassord = new SXReposFolders.FrmPassordAppFolder(this);
            _frmconfirm = new SXReposFolders.FrmConfirmAppFolder(this);
        }

#region Variables

#endregion

        /// <summary>
        /// The Self item info.
        /// </summary>
        [RepositoryItemInfo("f54ab49e-2074-43a3-bc83-bd6f67c47df2")]
        public virtual RepoItemInfo SelfInfo
        {
            get
            {
                return _selfInfo;
            }
        }

        /// <summary>
        /// The frmStartmeny folder.
        /// </summary>
        [RepositoryFolder("d63b8183-de21-4f47-b2c0-2a621be3654c")]
        public virtual SXReposFolders.FrmStartmenyAppFolder frmStartmeny
        {
            get { return _frmstartmeny; }
        }

        /// <summary>
        /// The frmPassord folder.
        /// </summary>
        [RepositoryFolder("1c05dda2-8483-4866-b5f1-e82345671435")]
        public virtual SXReposFolders.FrmPassordAppFolder frmPassord
        {
            get { return _frmpassord; }
        }

        /// <summary>
        /// The frmConfirm folder.
        /// </summary>
        [RepositoryFolder("f3f6b6d8-8481-4ffd-8675-b95efd2c1323")]
        public virtual SXReposFolders.FrmConfirmAppFolder frmConfirm
        {
            get { return _frmconfirm; }
        }
    }

    /// <summary>
    /// Inner folder classes.
    /// </summary>
    [System.CodeDom.Compiler.GeneratedCode("Ranorex", "8.2")]
    public partial class SXReposFolders
    {
        /// <summary>
        /// The FrmStartmenyAppFolder folder.
        /// </summary>
        [RepositoryFolder("d63b8183-de21-4f47-b2c0-2a621be3654c")]
        public partial class FrmStartmenyAppFolder : RepoGenBaseFolder
        {

            /// <summary>
            /// Creates a new frmStartmeny  folder.
            /// </summary>
            public FrmStartmenyAppFolder(RepoGenBaseFolder parentFolder) :
                    base("frmStartmeny", "/form[@title~'Startmeny' and @processname='Systemx']", parentFolder, 30000, null, false, "d63b8183-de21-4f47-b2c0-2a621be3654c", "")
            {
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("d63b8183-de21-4f47-b2c0-2a621be3654c")]
            public virtual Ranorex.Form Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.Form>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("d63b8183-de21-4f47-b2c0-2a621be3654c")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }
        }

        /// <summary>
        /// The FrmPassordAppFolder folder.
        /// </summary>
        [RepositoryFolder("1c05dda2-8483-4866-b5f1-e82345671435")]
        public partial class FrmPassordAppFolder : RepoGenBaseFolder
        {
            RepoItemInfo _txtusernameInfo;
            RepoItemInfo _txtpasswordInfo;

            /// <summary>
            /// Creates a new frmPassord  folder.
            /// </summary>
            public FrmPassordAppFolder(RepoGenBaseFolder parentFolder) :
                    base("frmPassord", "/form[@controlname='FPassord']", parentFolder, 30000, null, false, "1c05dda2-8483-4866-b5f1-e82345671435", "")
            {
                _txtusernameInfo = new RepoItemInfo(this, "txtUsername", "element[@controlname='UsrName']/text[@accessiblerole='Text']", 30000, null, "973e8024-dc92-4cc4-92a9-601be750374c");
                _txtpasswordInfo = new RepoItemInfo(this, "txtPassword", "element[@controlname='PWord']/text[@accessiblerole='Text']", 30000, null, "007ed0cb-b65d-4014-93f2-9c6f35d73d78");
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("1c05dda2-8483-4866-b5f1-e82345671435")]
            public virtual Ranorex.Form Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.Form>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("1c05dda2-8483-4866-b5f1-e82345671435")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }

            /// <summary>
            /// The txtUsername item.
            /// </summary>
            [RepositoryItem("973e8024-dc92-4cc4-92a9-601be750374c")]
            public virtual Ranorex.Text txtUsername
            {
                get
                {
                    return _txtusernameInfo.CreateAdapter<Ranorex.Text>(true);
                }
            }

            /// <summary>
            /// The txtUsername item info.
            /// </summary>
            [RepositoryItemInfo("973e8024-dc92-4cc4-92a9-601be750374c")]
            public virtual RepoItemInfo txtUsernameInfo
            {
                get
                {
                    return _txtusernameInfo;
                }
            }

            /// <summary>
            /// The txtPassword item.
            /// </summary>
            [RepositoryItem("007ed0cb-b65d-4014-93f2-9c6f35d73d78")]
            public virtual Ranorex.Text txtPassword
            {
                get
                {
                    return _txtpasswordInfo.CreateAdapter<Ranorex.Text>(true);
                }
            }

            /// <summary>
            /// The txtPassword item info.
            /// </summary>
            [RepositoryItemInfo("007ed0cb-b65d-4014-93f2-9c6f35d73d78")]
            public virtual RepoItemInfo txtPasswordInfo
            {
                get
                {
                    return _txtpasswordInfo;
                }
            }
        }

        /// <summary>
        /// The FrmConfirmAppFolder folder.
        /// </summary>
        [RepositoryFolder("f3f6b6d8-8481-4ffd-8675-b95efd2c1323")]
        public partial class FrmConfirmAppFolder : RepoGenBaseFolder
        {

            /// <summary>
            /// Creates a new frmConfirm  folder.
            /// </summary>
            public FrmConfirmAppFolder(RepoGenBaseFolder parentFolder) :
                    base("frmConfirm", "/form[@title='Bekreft']", parentFolder, 30000, null, false, "f3f6b6d8-8481-4ffd-8675-b95efd2c1323", "")
            {
            }

            /// <summary>
            /// The Self item.
            /// </summary>
            [RepositoryItem("f3f6b6d8-8481-4ffd-8675-b95efd2c1323")]
            public virtual Ranorex.Form Self
            {
                get
                {
                    return _selfInfo.CreateAdapter<Ranorex.Form>(true);
                }
            }

            /// <summary>
            /// The Self item info.
            /// </summary>
            [RepositoryItemInfo("f3f6b6d8-8481-4ffd-8675-b95efd2c1323")]
            public virtual RepoItemInfo SelfInfo
            {
                get
                {
                    return _selfInfo;
                }
            }
        }

    }
#pragma warning restore 0436
}