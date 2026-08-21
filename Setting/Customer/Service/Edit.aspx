<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" Inherits="Setting_Customer_Service_Edit" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Customer Service" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer">Customer</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer/service">Service</a></li>
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
                        <h4 class="card-title">Service Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Account</label>
                                        <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCustomer_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Service Name</label>
                                        <asp:DropDownList runat="server" ID="ddlService" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlService_SelectedIndexChanged"></asp:DropDownList>
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
                                        <label class="form-label">Description</label>
                                        <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" placeholder="Description ..." autocomplete="off"></asp:TextBox>
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblCustomerId"></asp:Label>
        <asp:Label runat="server" ID="lblReturnPage"></asp:Label>
    </div>

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>