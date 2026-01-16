<%@ Page Language="VB" AutoEventWireup="false" CodeFile="404.aspx.vb" Inherits="Error_404" Title="Error 404" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title><%: Page.Title %></title>

    <link href="https://fonts.googleapis.com/css2?family=Nunito:wght@300;400;600;700;800&display=swap" rel="stylesheet" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/bootstrap.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/vendors/bootstrap-icons/bootstrap-icons.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/app.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/pages/error.css") %>" />
    <link rel="icon" type="image/x-icon" href="<%: ResolveUrl("~/assets/images/logo/boos.ico") %>" />
</head>
<body>
    <form runat="server">
        <div id="error">
            <div class="error-page container">
                <div class="col-md-8 col-12 offset-md-2">
                    <img class="img-error" runat="server" src="~/Assets/images/error/error-404.png" alt="Not Found">
                    <div class="text-center">
                        <h1 class="error-title">NOT FOUND</h1>
                        <p class='fs-5 text-gray-600'>The page you are looking not found.</p>
                        <a runat="server" href="~/" class="btn btn-lg btn-outline-primary mt-3">Go Home</a>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
