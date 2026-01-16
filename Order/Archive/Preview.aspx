<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Preview.aspx.vb" Inherits="Order_Archive_Preview" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Preview</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Button runat="server" ID="btnBack" Text="Back" CssClass="btncopy" Font-Names="Calibri" Font-Size="Large" />
            <br /><br />
            <embed runat="server" id="embPrint" type="application/pdf" width="1500" height="900" />
        </div>
    </form>
</body>
</html>
