    <%@ Page Language="VB" AutoEventWireup="false" CodeFile="Password.aspx.vb" Inherits="Account_Password" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Change Password" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-lg-7 col-md-12 col-sm-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">New Password</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>New Password</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:TextBox runat="server" ID="txtPassword" TextMode="Password" CssClass="form-control" placeholder="New Password" autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Confirm Password</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:TextBox runat="server" ID="txtConfirmPassword" TextMode="Password" CssClass="form-control" placeholder="Confirm Password" autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">&nbsp;</div>
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <input class="form-check-input me-2" type="checkbox" value="" id="chkShowPass" onclick="togglePassword();">
                                            <label class="form-check-label text-gray-600" for="chkShowPass">
                                                Show Password
                                            </label>
                                        </div>
                                    </div>

                                    <div class="row" runat="server" id="divError">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card-footer text-start">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalSuccess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Successfully</h5>
                </div>
                <div class="modal-body">
                    Your password succesfully updated
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" id="vieworder" class="btn btn-success w-100" data-bs-dismiss="modal">Home Page</a>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function togglePassword() {
            var password = document.getElementById('<%= txtPassword.ClientID %>');
            var confirm = document.getElementById('<%= txtConfirmPassword.ClientID %>');
            var checkBox = document.getElementById('chkShowPass');

            if (checkBox.checked) {
                password.type = "text";
                confirm.type = "text";
            } else {
                password.type = "password";
                confirm.type = "password";
            }
        }

        document.getElementById("modalSuccess").addEventListener("hide.bs.modal", function () {
            document.activeElement.blur();
            document.body.focus();
        });
        function showSuccess() {
            $('#modalSuccess').modal('show');
        }

        $(document).on('hidden.bs.modal', '#modalSuccess', function () {
            window.location.href = "/";
        });

        $("#vieworder").on("click", () => window.location.href = "/");
    </script>
</asp:Content>