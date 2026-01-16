<%@ Page Language="VB" AutoEventWireup="false" CodeFile="AddCSV.aspx.vb" Inherits="Order_AddCSV" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Order | CSV" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/order">Order</a></li>
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
                        <h4 class="card-title">Import CSV</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row mb-3">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label class="form-label">Customer Account</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label class="form-label">Method</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                                            <asp:DropDownList runat="server" ID="ddlMethod" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlMethod_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="XLSX" Text="XLSX"></asp:ListItem>
                                                <asp:ListItem Value="API" Text="API"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row" runat="server" id="divApi">
                                        <div class="col-12 col-sm-12 col-lg-3" >
                                            <label class="form-label">Order ID</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <asp:TextBox runat="server" ID="txtApiId" CssClass="form-control" placeholder="Order ID ..." autocomplete="off"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="row" runat="server" id="divXls">
                                        <div class="col-12 col-sm-12 col-lg-3" >
                                            <label class="form-label">Upload CSV Order</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-9 form-group">
                                            <asp:FileUpload runat="server" ID="fuFile" CssClass="form-control" />
                                        </div>
                                    </div>
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
                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnSubmit" CssClass="btn btn-primary" Text="Submit" OnClick="btnSubmit_Click" />
                        <asp:Button runat="server" ID="btnCancel" CssClass="btn btn-danger" Text="Cancel" OnClick="btnCancel_Click" />
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>