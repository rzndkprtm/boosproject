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
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Detail Validation Form</h4>
                    </div>
                    <div class="card-body">
                        <div class="row mb-3">
                            <div class="col-12 form-group">
                                <label class="form-label">Validation</label>
                                <asp:DropDownList runat="server" ID="ddlValidation" CssClass="choices form-select"></asp:DropDownList>
                            </div>
                        </div>
                        <asp:Repeater ID="rptDetail" runat="server" OnItemDataBound="rptDetail_ItemDataBound">
                            <ItemTemplate>
                                <div class="border rounded p-3 mb-3 bg-light">
                                    <div class="row mt-3 mb-2">
                                        <div class="col-lg-1 form-group">
                                            <label>Group No</label>
                                            <asp:DropDownList ID="ddlGroupNo" runat="server" CssClass="choices form-select">
                                                <asp:ListItem Value=""></asp:ListItem>
                                                <asp:ListItem Value="1">1</asp:ListItem>
                                                <asp:ListItem Value="2">2</asp:ListItem>
                                                <asp:ListItem Value="3">3</asp:ListItem>
                                                <asp:ListItem Value="4">4</asp:ListItem>
                                                <asp:ListItem Value="5">5</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-lg-3 form-group">
                                            <label>Field Name</label>
                                            <asp:DropDownList ID="ddlFieldName" runat="server" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-lg-3 form-group">
                                            <label>Operator</label>
                                            <asp:DropDownList ID="ddlOperator" runat="server" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                        <div class="col-lg-3 form-group">
                                            <label>Compare Value</label>
                                            <asp:TextBox ID="txtCompareValue" runat="server" CssClass="form-control" Height="45px"></asp:TextBox>
                                        </div>
                                        <div class="col-lg-2 form-group">
                                            <label>Data Type</label>
                                            <asp:DropDownList ID="ddlDataType" runat="server" CssClass="choices form-select">
                                                <asp:ListItem Value=""></asp:ListItem>
                                                <asp:ListItem Value="STRING">STRING</asp:ListItem>
                                                <asp:ListItem Value="INTEGER">INTEGER</asp:ListItem>
                                                <asp:ListItem Value="DECIMAL">DECIMAL</asp:ListItem>
                                                <asp:ListItem Value="DATE">DATE</asp:ListItem>
                                                <asp:ListItem Value="BOOLEAN">BOOLEAN</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="text-end">
                                        <asp:LinkButton ID="btnDelete" runat="server" CssClass="btn btn-sm btn-danger" CommandArgument="<%# Container.ItemIndex %>" OnClick="btnDelete_Click">Delete</asp:LinkButton>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <div class="row mb-2">
                            <div class="col-12 text-start">
                                <asp:Button ID="btnAddRow" runat="server" CssClass="btn btn-success" Text="+ Add Rule" OnClick="btnAddRow_Click" />
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
                    <div class="card-footer text-start">
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