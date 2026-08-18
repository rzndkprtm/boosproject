<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Price_Service_Add" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Price Service" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/product">Product Group</a></li>
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
                        <h4 class="card-title">Product Group Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="row mb-2">
                                            <div class="col-12 col-sm-12 col-lg-5 mb-2 form-group">
                                                <label class="form-label">Design Type</label>
                                                <asp:ListBox runat="server" ID="lbPriceGroup" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
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
                                                <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                                    <asp:ListItem Value="" Text=""></asp:ListItem>
                                                    <asp:ListItem Value="Price" Text="Price"></asp:ListItem>
                                                    <asp:ListItem Value="Formula" Text="Formula"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Default Buy Price</label>
                                                <asp:TextBox runat="server" ID="txtBuyPrice" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                            <div class="col-12 col-sm-12 col-lg-4 form-group">
                                                <label class="form-label">Default Sell Price</label>
                                                <asp:TextBox runat="server" ID="txtSellPrice" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>