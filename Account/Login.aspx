<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Login.aspx.vb" Inherits="Account_Login" %>

<!DOCTYPE html>

<html lang="en">

<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login - BOOS</title>
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/bootstrap.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/vendors/bootstrap-icons/bootstrap-icons.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/app.css") %>" />
    <link rel="stylesheet" href="<%: ResolveUrl("~/Assets/css/pages/auth.css") %>" />
    <link rel="icon" type="image/x-icon" href="<%: ResolveUrl("~/assets/images/logo/boos.ico") %>" />
</head>

<body>
    <form id="formLogin" runat="server">
        <div id="auth">
            <div class="row h-100">
                <div class="col-lg-5 col-12">
                    <div id="auth-left">
                        <h1 class="auth-title">Log In.</h1>
                        <p class="auth-subtitle mb-5">Please log in using the information provided by Customer Service.</p>
                        <div class="form-group position-relative has-icon-left mb-4">
                            <asp:TextBox runat="server" ID="txtUserLogin" CssClass="form-control form-control-xl" placeholder="User Name" autocomplete="off"></asp:TextBox>
                            <div class="form-control-icon">
                                <i class="bi bi-person"></i>
                            </div>
                        </div>
                        <div class="form-group position-relative has-icon-left mb-2">
                            <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control form-control-xl" placeholder="Your password" autocomplete="off"></asp:TextBox>
                            <div class="form-control-icon">
                                <i class="bi bi-shield-lock"></i>
                            </div>
                        </div>

                        <div class="form-check form-check-lg d-flex align-items-end">
                            <input class="form-check-input me-2" type="checkbox" value="" id="chkShowPass" onclick="togglePassword();">
                            <label class="form-check-label text-gray-600" for="chkShowPass">
                                Show Password
                            </label>
                        </div>

                        <div class="alert alert-danger mt-4" runat="server" id="divError">
                            <span runat="server" id="msgError"></span>
                        </div>

                        <asp:Button runat="server" ID="btnLogin" CssClass="btn btn-primary btn-block btn-lg shadow-lg mt-5" Text="Log In" OnClick="btnLogin_Click" />

                        <div class="text-center mt-3 text-lg fs-4">
                            <%--<p class="text-gray-600">Don't have an account? <a href="auth-register.html" class="font-bold">Sign up</a>.</p>--%>
                            <p>
                                <a runat="server" class="font-bold" href="~/account/forgot">Forgot password?</a>
                            </p>
                        </div>
                    </div>
                </div>

                <div class="col-lg-7 d-none d-lg-block">
                    <div id="auth-right"></div>
                </div>
            </div>
        </div>

        <div runat="server" visible="false">
            <asp:Label runat="server" ID="lblDeviceId"></asp:Label>
        </div>

        <script type="text/javascript">
            function togglePassword() {
                var password = document.getElementById('<%= txtPassword.ClientID %>');
                var checkBox = document.getElementById('chkShowPass');

                if (checkBox.checked) {
                    password.type = "text";
                } else {
                    password.type = "password";
                }
            }
        </script>
    </form>
</body>

</html>