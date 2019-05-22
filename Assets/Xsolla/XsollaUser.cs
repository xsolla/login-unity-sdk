using System;

namespace Xsolla
{
    // REVIEW Json classes can be moved to separate files as well - its a good practice to have separate files for different classes.

    #region JsonClasses
    [Serializable]
    public struct XsollaUser
    {
        public string exp;
        public string iss;
        public string iat;
        public string username;
        public string xsolla_login_access_key;
        public string sub;
        public string email;
        public string xsolla_login_project_id;
        public string publisher_id;
        public string provider;
        public string name;
        public bool is_linked;
    }
#endregion
}