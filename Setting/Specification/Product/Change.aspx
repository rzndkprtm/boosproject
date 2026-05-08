<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Change.aspx.vb" Inherits="Setting_Specification_Product_Change" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Product Change" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification">Specification</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/product">Product</a></li>
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
                        <h4 class="card-title">Change Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Design Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <asp:DropDownList runat="server" ID="ddlDesignType" CssClass="choices form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlDesignType_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Blind Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-8 form-group">
                                            <asp:DropDownList runat="server" ID="ddlBlindType" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Tube Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <asp:DropDownList runat="server" ID="ddlTubeType" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Control Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <asp:DropDownList runat="server" ID="ddlControlType" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Colour Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <asp:DropDownList runat="server" ID="ddlColourType" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label>Status</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <asp:DropDownList runat="server" ID="ddlStatus" CssClass="choices form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                                <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                                <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                                <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
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
            <div class="col-12 col-sm-12 col-lg-5"></div>
        </section>
    </div>
</asp:Content>