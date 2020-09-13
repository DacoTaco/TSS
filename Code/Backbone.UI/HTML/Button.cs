using System;
using System.Web.UI.HtmlControls;

namespace TechnicalServiceSystem.UI.HTML
{
    public class Button : HtmlButton
    {
        public Button(){ }

        //when rendering the button we will inject the btn class to the button
        protected override void OnPreRender(EventArgs e)
        {
            SetAttribute("class", "btn " + GetAttribute("class"));
            base.OnPreRender(e);
        }
    }
}
