<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Price_Service_Edit" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Edit Price Service" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/service">Price Service</a></li>
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
                        <h4 class="card-title">Price Service Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Sub Company</label>
                                                <asp:ListBox runat="server" ID="lbCompanyDetail" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Name</label>
                                                <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Type</label>
                                                <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlType_SelectedIndexChanged">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Price" Text="Price"></asp:ListItem>
                                                    <asp:ListItem Value="Formula" Text="Formula"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group" runat="server" id="divDefaultBuy">
                                                <label class="form-label">Default Buy Price</label>
                                                <asp:TextBox runat="server" ID="txtBuyPrice" CssClass="form-control" placeholder="Default Buy Price ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group" runat="server" id="divDefaultSell">
                                                <label class="form-label">Default Sell Price</label>
                                                <asp:TextBox runat="server" ID="txtSellPrice" CssClass="form-control" placeholder="Default Sell Price ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Parameter</label>
                                                <asp:DropDownList runat="server" ID="ddlParameter" CssClass="form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="ItemQty" Text="ItemQty"></asp:ListItem>
                                                    <asp:ListItem Value="TotalSQM" Text="TotalSQM"></asp:ListItem>
                                                    <asp:ListItem Value="TotalLM" Text="TotalLM"></asp:ListItem>
                                                    <asp:ListItem Value="TotalBuy" Text="TotalBuy"></asp:ListItem>
                                                    <asp:ListItem Value="TotalSell" Text="TotalSell"></asp:ListItem>
                                                    <asp:ListItem Value="OrderValue" Text="OrderValue"></asp:ListItem>
                                                    <asp:ListItem Value="Distance" Text="Distance"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2" runat="server" id="divFormula">
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Operator</label>
                                                <asp:DropDownList runat="server" ID="ddlOperator" CssClass="form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="+" Text="Add (+)"></asp:ListItem>
                                                    <asp:ListItem Value="-" Text="Subtract (-)"></asp:ListItem>
                                                    <asp:ListItem Value="*" Text="Multiply (×)"></asp:ListItem>
                                                    <asp:ListItem Value="/" Text="Divide (÷)"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Buy Value</label>
                                                <asp:TextBox runat="server" ID="txtBuyValue" CssClass="form-control" placeholder="Buy Value ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Sell Value</label>
                                                <asp:TextBox runat="server" ID="txtSellValue" CssClass="form-control" placeholder="Sell Value ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Minimum Value</label>
                                                <asp:TextBox runat="server" ID="txtMinimumValue" CssClass="form-control" placeholder="Minimum Value ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Maximum Value</label>
                                                <asp:TextBox runat="server" ID="txtMaximumValue" CssClass="form-control" placeholder="Maximum Value ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Region</label>
                                                <asp:DropDownList runat="server" ID="ddlRegion" CssClass="choices form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="NSW" Text="NSW"></asp:ListItem>
                                                    <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                                                    <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                                                    <asp:ListItem Value="TAS" Text="TAS"></asp:ListItem>
                                                    <asp:ListItem Value="NT" Text="NT"></asp:ListItem>
                                                    <asp:ListItem Value="ACT" Text="ACT"></asp:ListItem>
                                                    <asp:ListItem Value="VIC" Text="VIC"></asp:ListItem>
                                                    <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
                                                    <asp:ListItem Value="JKT" Text="JKT"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Auto Create</label>
                                                <asp:DropDownList runat="server" ID="ddlAutoCreate" CssClass="choices form-select">
                                                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Allow Custom</label>
                                                <asp:DropDownList runat="server" ID="ddlAllowCustom" CssClass="choices form-select">
                                                    <asp:ListItem Value="0" Text="No"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 form-group">
                                                <label class="form-label">Description</label>
                                                <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" placeholder="Description ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-6 form-group">
                                                <label class="form-label">Status</label>
                                                <asp:DropDownList runat="server" ID="ddlStatus" CssClass="choices form-select">
                                                    <asp:ListItem Value="Active" Text="Active"></asp:ListItem>
                                                    <asp:ListItem Value="Inactive" Text="Inactive"></asp:ListItem>
                                                </asp:DropDownList>
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAutoCreate"></asp:Label>
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

                var isMultiple = el.multiple;

                el.choices = new Choices(el, {
                    searchEnabled: true,
                    itemSelectText: '',
                    shouldSort: false,
                    removeItemButton: isMultiple
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
