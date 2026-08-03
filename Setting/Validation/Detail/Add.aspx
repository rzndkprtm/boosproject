<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Add.aspx.vb" Inherits="Setting_Validation_Detail_Add" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Add Detail Validation" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/validation">Validation</a></li>
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
                        <h4 class="card-title">Price Base Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="form form-vertical">
                            <div class="form-body">
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-9 form-group">
                                        <label class="form-label">Validation Name</label>
                                        <asp:DropDownList runat="server" ID="ddlValidation" CssClass="choices form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-3 form-group">
                                        <label class="form-label">Group No</label>
                                        <asp:DropDownList runat="server" ID="ddlCategory" CssClass="choices form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Field Name</label>
                                        <asp:DropDownList runat="server" ID="ddlFieldName" CssClass="choices form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Operator</label>
                                        <asp:DropDownList runat="server" ID="ddlOperator" CssClass="choices form-select"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Compare Value</label>
                                        <asp:TextBox runat="server" ID="txtCompareValue" CssClass="form-control" placeholder="Compare Value ..."></asp:TextBox>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12 form-group">
                                        <label class="form-label">Data Type</label>
                                        <asp:DropDownList runat="server" ID="ddlDataType" CssClass="choices form-select"></asp:DropDownList>
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
            </div>
        </section>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>
</asp:Content>