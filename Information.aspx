<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Information.aspx.vb" Inherits="Information" %>

<!DOCTYPE html>

<html lang="en">
    
    <head runat="server">
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Information - BOOS</title>
        <link href="https://fonts.googleapis.com/css2?family=Nunito:wght@300;400;600;700;800&display=swap" rel="stylesheet">

        <link rel="stylesheet" href="<%: ResolveUrl("~/assets/css/bootstrap.css") %>?v=<%= DateTime.Now.Ticks %>" />
        <link rel="stylesheet" href="<%: ResolveUrl("~/assets/vendors/iconly/bold.css") %>?v=<%= DateTime.Now.Ticks %>" />
        <link rel="stylesheet" href="<%: ResolveUrl("~/assets/vendors/perfect-scrollbar/perfect-scrollbar.css") %>?v=<%= DateTime.Now.Ticks %>" />
        <link rel="stylesheet" href="<%: ResolveUrl("~/assets/vendors/bootstrap-icons/bootstrap-icons.css") %>?v=<%= DateTime.Now.Ticks %>" />
        <link rel="stylesheet" href="<%: ResolveUrl("~/assets/css/app.css") %>?v=<%= DateTime.Now.Ticks %>" />
        <link rel="icon" type="image/x-icon" href="<%: ResolveUrl("~/assets/images/logo/boos.ico") %>?v=<%= DateTime.Now.Ticks %>" />
    </head>

    <body>
        <form runat="server">
            <div id="app">
                <div id="main" class="layout-horizontal">
                    <header class="mb-5">
                        <div class="header-top">
                            <div class="container">
                                <div class="logo">
                                    <a runat="server" href="~/">
                                        <asp:Image runat="server" ID="imgLogo" ImageUrl="~/Assets/images/logo/general.jpg" AlternateText="BOOS" />
                                    </a>
                                </div>
                                <div class="header-top-right">
                                    <a href="#" class="burger-btn d-block d-xl-none">
                                        <i class="bi bi-justify fs-3"></i>
                                    </a>
                                </div>
                            </div>
                        </div>

                        <nav class="main-navbar">
                            <div class="container">
                                <ul>
                                    <li class="menu-item">
                                        <a runat="server" href="~/" class='menu-link'>
                                            <i class="bi bi-house-door-fill"></i>
                                            <span>Home</span>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </nav>
                    </header>

                    <div class="content-wrapper container">
                        <div class="page-heading">
                            <h3>Information</h3>
                        </div>
                        <div class="page-content">
                            <section class="row">
                                <div class="col-12">
                                    <div class="card">
                                        <div class="card-content">
                                            <div class="card-body">
                                                <div class="row">
                                                    <div class="col-12">
                                                        <p class="card-text text-xl">
                                                            Hi <span class="font-bold" runat="server" id="spanHi"></span>,
                                                            <br /><br />
                                                            We apologize for any inconvenience caused. We are currently directing you to use our new website for new orders.
                                                            <br /><br />
                                                            For future use, please access the system directly via the following link:
                                                            <br />
                                                            <a href="https://bigblinds.ordersblindonline.com">bigblinds.ordersblindonline.com</a>
                                                            <br /><br />
                                                            Below are your login details:
                                                        </p>
                                                        <asp:Label runat="server" ID="lblUid" Visible="false"></asp:Label>
                                                        <p class="card-text text-xl">
                                                            User : <span class="font-bold" runat="server" id="spanUser"></span>
                                                            <br />
                                                            Password : <span class="font-bold" runat="server" id="spanPassword"></span>
                                                        </p>
                                                        <br />
                                                        <p class="card-text text-xl">
                                                            Should you have any questions or require further assistance, please feel free to contact me.
                                                        </p>
                                                        <br /><br />
                                                        <p class="card-text text-xl">
                                                            Kind Regards,
                                                            <br /><br /><br />
                                                            <b>Reza Andika Pratama</b> | Developer
                                                            <br />
                                                            Email : <a href="mailto:reza@bigblinds.co.id">reza@bigblinds.co.id</a>
                                                            <br />
                                                            WhatsApp : <a runat="server" id="lnkWhatsapp" target="_blank">0852 1504 3355</a>
                                                        </p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="card-footer d-flex justify-content-between">
                                            <asp:Button runat="server" ID="btnLogin" CssClass="btn btn-primary btn-block" Text="Login Here" OnClick="btnLogin_Click" />
                                        </div>
                                    </div>
                                </div>
                            </section>
                        </div>
                        
                    </div>

                    <footer class="mt-3">
                        <div class="container">
                            <div class="footer clearfix mb-0 text-muted">
                                <div class="float-end">
                                    <p>2026 &copy; BOOS</p>
                                </div>
                            </div>
                        </div>
                    </footer>
                </div>
            </div>

            <script src="<%: ResolveUrl("~/assets/vendors/perfect-scrollbar/perfect-scrollbar.min.js") %>?v=<%= DateTime.Now.Ticks %>"></script>
            <script src="<%: ResolveUrl("~/assets/js/bootstrap.bundle.min.js") %>?v=<%= DateTime.Now.Ticks %>"></script>
            <script src="<%: ResolveUrl("~/assets/vendors/choices.js/choices.min.js") %>?v=<%= DateTime.Now.Ticks %>"></script>
            <script src="<%: ResolveUrl("~/assets/js/pages/form-element-select.js") %>?v=<%= DateTime.Now.Ticks %>"></script>
            <script src="<%: ResolveUrl("~/assets/js/pages/horizontal-layout.js") %>?v=<%= DateTime.Now.Ticks %>"></script>
        </form>
        
    </body>
</html>