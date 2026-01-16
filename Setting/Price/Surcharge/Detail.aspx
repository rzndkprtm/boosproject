<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Price_Surcharge_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Price Surcharge Detail" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/surcharge">Surcharge</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Add Form</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row mb-2">
                                        <div class="col-6 form-group">
                                            <label>Design Type</label>
                                            <asp:DropDownList runat="server" ID="ddlDesign" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesign_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                        <div class="col-6 form-group">
                                            <label>Blind Type</label>
                                            <asp:DropDownList runat="server" ID="ddlBlind" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label>Surcharge Name</label>
                                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Surcharge Name ...." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-3 form-group">
                                            <label>Item Number</label>
                                            <asp:DropDownList runat="server" ID="ddlBlindNumber" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="1" Text="1"></asp:ListItem>
                                                <asp:ListItem Value="2" Text="2"></asp:ListItem>
                                                <asp:ListItem Value="3" Text="3"></asp:ListItem>
                                                <asp:ListItem Value="4" Text="4"></asp:ListItem>
                                                <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                                <asp:ListItem Value="6" Text="6"></asp:ListItem>
                                                <asp:ListItem Value="7" Text="7"></asp:ListItem>
                                                <asp:ListItem Value="8" Text="8"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-5 form-group">
                                            <label>Field Name</label>
                                            <asp:DropDownList runat="server" ID="ddlFieldName" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-4 form-group">
                                            <label>Price Group</label>
                                            <asp:DropDownList runat="server" ID="ddlPriceGroup" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label>Formula</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtFormula" Height="100px" CssClass="form-control" placeholder="Formula ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-6 form-group">
                                            <label>Buy Charge</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtBuyCharge" Height="150px" CssClass="form-control" placeholder="Buy Charge ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                        <div class="col-6 form-group">
                                            <label>Sell Charge</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtSellCharge" Height="150px" CssClass="form-control" placeholder="Sell Charge ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-12 form-group">
                                            <label>Description</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="row mb-2">
                                        <div class="col-3 form-group">
                                            <label>Active</label>
                                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
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
                    </div>

                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>
</asp:Content>
