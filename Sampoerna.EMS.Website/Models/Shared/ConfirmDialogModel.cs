using System;

namespace Sampoerna.EMS.Website.Models.Shared
{
    public class ConfirmDialogModel
    {
        public class Button
        {
            /// <summary>
            /// Property represents label of button
            /// </summary>
            public String Label { set; get; }

            /// <summary>
            /// Property represents id of button
            /// </summary>
            public String Id { set; get; }

            /// <summary>
            /// Property represents css class of button
            /// </summary>
            public String CssClass { set; get; }
        }
        /// <summary>
        /// Property represents confirm dialog id
        /// </summary>
        public String Id { set; get; }

        /// <summary>
        /// Property represents css class used in modal dialog
        /// </summary>
        public String CssClass { set; get; }

        /// <summary>
        /// Property represents name of modal label
        /// </summary>
        public String ModalLabel { set; get; }
        
        /// <summary>
        /// Property represents modal title
        /// </summary>
        public String Title { set; get; }

        /// <summary>
        /// Property represents message to display in modal dialog
        /// </summary>
        public String Message { set; get; }

        /// <summary>
        /// Property represents action button
        /// </summary>
        public Button Action { set; get; }

    }
}