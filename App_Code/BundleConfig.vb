
Public Module BundleConfig
    ' For more information on Bundling, visit https://go.microsoft.com/fwlink/?LinkID=303951
    Public Sub RegisterBundles(bundles As BundleCollection)
        bundles.Add(New ScriptBundle("~/bundles/WebFormsJs").Include(
            "~/Scripts/WebForms/WebForms.js",
            "~/Scripts/WebForms/WebUIValidation.js",
            "~/Scripts/WebForms/MenuStandards.js",
            "~/Scripts/WebForms/Focus.js", "~/Scripts/WebForms/GridView.js",
            "~/Scripts/WebForms/DetailsView.js",
            "~/Scripts/WebForms/TreeView.js",
            "~/Scripts/WebForms/WebParts.js"))

        ' Order is very important for these files to work, they have explicit dependencies
        bundles.Add(New ScriptBundle("~/bundles/MsAjaxJs").Include(
            "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
            "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"))

        ' Use the Development version of Modernizr to develop with and learn from. Then, when you’re
        ' ready for production, use the build tool at https://modernizr.com to pick only the tests you need
        bundles.Add(New ScriptBundle("~/bundles/modernizr").Include(
            "~/Scripts/modernizr-*"))

        RegisterJQueryScriptManager()
    End Sub

    Public Sub RegisterJQueryScriptManager()
        Dim jQueryScriptResourceDefinition As New ScriptResourceDefinition
        With jQueryScriptResourceDefinition
            .Path = "~/scripts/jquery-3.7.1.min.js"
            .DebugPath = "~/scripts/jquery-3.7.1.js"
            .CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.1.min.js"
            .CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.1.js"
        End With

        ScriptManager.ScriptResourceMapping.AddDefinition("jquery", jQueryScriptResourceDefinition)
    End Sub
End Module
