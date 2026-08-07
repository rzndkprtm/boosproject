<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Price_Calculation_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Calculation" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/calculation">Calculation</a></li>
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
                        <h4 class="card-title">Price Calculation Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Name</label>
                                                <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mt-2">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Price Group</label>
                                                <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlPriceGroup_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Design Type</label>
                                                <asp:DropDownList runat="server" ID="ddlDesignType" CssClass="choices form-select"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-3">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Method</label>
                                                <asp:DropDownList runat="server" ID="ddlMethod" CssClass="choices form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Square Metre" Text="Square Metre"></asp:ListItem>
                                                    <asp:ListItem Value="Linear Metre" Text="Linear Metre"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Formula</label>
                                                <asp:DropDownList runat="server" ID="ddlFormula" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlFormula_SelectedIndexChanged">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="SQM" Text="SQM"></asp:ListItem>
                                                    <asp:ListItem Value="SQM_MIN" Text="SQM_MIN"></asp:ListItem>
                                                    <asp:ListItem Value="SQM_ROUND" Text="SQM_ROUND"></asp:ListItem>
                                                    <asp:ListItem Value="LM" Text="LM"></asp:ListItem>
                                                    <asp:ListItem Value="LM_MIN" Text="LM_MIN"></asp:ListItem>
                                                    <asp:ListItem Value="CUT_LENGTH" Text="CUT_LENGTH"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row" runat="server" id="divMinimumSize">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Sell Minimum Size</label>
                                                <asp:TextBox runat="server" ID="txtSellMinSize" CssClass="form-control" placeholder="Sell Minimum Size ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Buy Minimum Size</label>
                                                <asp:TextBox runat="server" ID="txtBuyMinSize" CssClass="form-control" placeholder="Buy Minimum Size ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row" runat="server" id="divMinimumWidth">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Sell Minimum Width</label>
                                                <asp:TextBox runat="server" ID="txtSellMinWidth" CssClass="form-control" placeholder="Sell Minimum Width ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Buy Minimum Width</label>
                                                <asp:TextBox runat="server" ID="txtBuyMinWidth" CssClass="form-control" placeholder="Buy Minimum Width ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row" runat="server" id="divMinimumHeight">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Sell Minimum Height</label>
                                                <asp:TextBox runat="server" ID="txtSellMinDrop" CssClass="form-control" placeholder="Sell Minimum Height ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Buy Minimum Height</label>
                                                <asp:TextBox runat="server" ID="txtBuyMinDrop" CssClass="form-control" placeholder="Buy Minimum Height ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Active</label>
                                                <asp:DropDownList runat="server" ID="ddlActive" CssClass="choices form-select">
                                                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
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
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
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
        eState(null, null, window.location.href);
    </script>
</asp:Content>