<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Specification_Fabric_Colour_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Fabric Colour" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/fabric">Fabric</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-6">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Fabric Colour Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Fabric Type</label>
                                        <asp:DropDownList runat="server" ID="ddlFabricType" CssClass="choices form-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Sub Company</label>
                                        <asp:ListBox runat="server" ID="lbCompanyDetail" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">BOE ID</label>
                                        <asp:TextBox runat="server" ID="txtBoeId" CssClass="form-control" placeholder="BOE ID ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">INVENTORY ID</label>
                                        <asp:TextBox runat="server" ID="txtInventoryId" CssClass="form-control" placeholder="Inventory ID ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Factory</label>
                                        <asp:DropDownList runat="server" ID="ddlFactory" CssClass="choices form-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Colour</label>
                                        <asp:TextBox runat="server" ID="txtColour" CssClass="form-control" placeholder="Colour ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Width</label>
                                        <asp:TextBox runat="server" ID="txtWidth" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Roll Qty</label>
                                        <asp:TextBox runat="server" ID="txtRollQty" CssClass="form-control" placeholder="Roll Qty ..." autocomplete="off"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">ETA Factory</label>
                                        <asp:TextBox runat="server" ID="txtETAFactory" TextMode="Date" CssClass="form-control" placeholder="ETA Factory ..." autocomplete="off"></asp:TextBox>
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
                                        <label class="form-label">Status</label>
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="choices form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                            <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                            <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                            <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                                        </asp:DropDownList>
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
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" />
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>