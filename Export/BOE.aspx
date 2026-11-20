<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.SqlClient" %>
<%@ Page Language="VB" Title="Export BOE Result" ContentType="text/xml" Debug="true" %>

<script runat="server">
    Protected Sub Page_Load(sender As Object, e As EventArgs)
        Response.Redirect("https://boe.ordersblindonline.com/export.aspx" & Request.Url.Query, False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
</script>