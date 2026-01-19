
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

        'bundles.Add(New StyleBundle("~/bundles/booscss").Include(
        '    "~/Assets/vendors/choices.js/choices.min.css",
        '    "~/Assets/css/bootstrap.css",
        '    "~/Assets/vendors/sweetalert2/sweetalert2.min.css",
        '    "~/Assets/vendors/iconly/bold.css",
        '    "~/Assets/vendors/perfect-scrollbar/perfect-scrollbar.css",
        '    "~/Assets/vendors/bootstrap-icons/bootstrap-icons.css",
        '    "~/Assets/css/app.css"))

        'bundles.Add(New ScriptBundle("~/bundles/boosjs").Include(
        '    "~/Assets/vendors/perfect-scrollbar/perfect-scrollbar.min.js",
        '    "~/Assets/js/bootstrap.bundle.min.js",
        '    "~/Assets/vendors/sweetalert2/sweetalert2.all.min.js",
        '    "~/Assets/vendors/choices.js/choices.min.js",
        '    "~/Assets/js/pages/form-element-select.js",
        '    "~/Assets/js/pages/horizontal-layout.js"))

        RegisterJQueryScriptManager()
    End Sub

    Public Sub RegisterJQueryScriptManager()
        Dim jQueryScriptResourceDefinition As New ScriptResourceDefinition
        With jQueryScriptResourceDefinition
            .Path = "~/Scripts/jquery-3.7.1.min.js"
            .DebugPath = "~/Scripts/jquery-3.7.1.js"
            .CdnPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.1.min.js"
            .CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-3.7.1.js"
        End With

        ScriptManager.ScriptResourceMapping.AddDefinition("jquery", jQueryScriptResourceDefinition)
    End Sub
End Module
