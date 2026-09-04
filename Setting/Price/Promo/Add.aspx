<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Price_Promo_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Promo" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/promo">Promo</a></li>
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
                        <h4 class="card-title">Promo Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Type</label>
                                                <asp:DropDownList runat="server" ID="ddlType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Sell" Text="Promo Sell"></asp:ListItem>
                                                    <asp:ListItem Value="Buy" Text="Promo Buy"></asp:ListItem>
                                                    <asp:ListItem Value="Factory" Text="Promo Factory"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Name</label>
                                                <asp:TextBox runat="server" ID="txtName" CssClass="form-control" Height="45px" placeholder="Promo Name ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Data Type</label>
                                                <asp:DropDownList runat="server" ID="ddlDataType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDataType_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-8 form-group">
                                                <label class="form-label">Data Name</label>
                                                <asp:DropDownList runat="server" ID="ddlDataId" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Start Date</label>
                                                <asp:TextBox runat="server" ID="txtStartDate" TextMode="Date" CssClass="form-control" placeholder="Start Date ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">End Date</label>
                                                <asp:TextBox runat="server" ID="txtEndDate" TextMode="Date" CssClass="form-control" placeholder="Start Date ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Status</label>
                                                <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                                    <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                                    <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Description</label>
                                                <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
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
                    shouldSort: false,
                    searchResultLimit: 50
                });
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            initChoices();
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
