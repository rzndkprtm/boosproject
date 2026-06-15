<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Login_User_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Login User" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/login">Login</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/login/user">Login User</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Login Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <div class="row mb-2">
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <label class="form-label">Role</label>
                                                    <asp:DropDownList runat="server" ID="ddlRole" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlRole_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                                <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                    <label class="form-label">Level</label>
                                                    <asp:DropDownList runat="server" ID="ddlLevel" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="row mb-2" runat="server" id="divCustomer">
                                                <div class="col-12 form-group">
                                                    <label class="form-label">Customer Account</label>
                                                    <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="choices form-select"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">UserName</label>
                                            <asp:TextBox runat="server" ID="txtAddUserName" CssClass="form-control" placeholder="UserName ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">Password</label>
                                            <asp:TextBox runat="server" ID="txtAddPassword" TextMode="Password" CssClass="form-control" placeholder="Password ..." autocomplete="new-password"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">Full Name</label>
                                            <asp:TextBox runat="server" ID="txtFullName" CssClass="form-control" placeholder="Full Name ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group" runat="server" id="divEmail">
                                            <label class="form-label">Email</label>
                                            <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row mb-2">
                                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                                            <label class="form-label">Pricing</label>
                                            <asp:DropDownList runat="server" ID="ddlPricing" CssClass="form-select">
                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row mt-3" runat="server" id="divError">
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
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title text-center">Information</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body"></div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });

        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
                initChoices();
            });
        }

        function initChoices() {
            document.querySelectorAll("select.choices").forEach(function (el) {
                if (el.choices) {
                    el.choices.destroy();
                }

                el.choices = new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false
                });
            });
        }

        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });
    </script>
</asp:Content>