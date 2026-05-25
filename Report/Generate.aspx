<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Generate.aspx.vb" Inherits="Report_Generate" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Generate Report" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/report">Report</a></li>
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
                        <h4 class="card-title">Generate Form</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 form-group">
                                            <label class="form-label">Company</label>
                                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="choices form-select"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">Data Type</label>
                                            <asp:DropDownList runat="server" ID="ddlDataType" CssClass="choices form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Fabric (List)" Text="Fabric (List)"></asp:ListItem>
                                                <asp:ListItem Value="Fabric (Order)" Text="Fabric (Order)"></asp:ListItem>
                                                <asp:ListItem Value="Fabric Colour (List)" Text="Fabric Colour (List)"></asp:ListItem>
                                                <asp:ListItem Value="Fabric Colour (Order)" Text="Fabric Colour (Order)"></asp:ListItem>
                                                <asp:ListItem Value="Customer (List)" Text="Customer (List)"></asp:ListItem>
                                                <asp:ListItem Value="Customer (Order)" Text="Customer (Order)"></asp:ListItem>
                                                <asp:ListItem Value="Job Order" Text="Job Order"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                                            <label class="form-label">File Type</label>
                                            <asp:DropDownList runat="server" ID="ddlFileType" CssClass="choices form-select">
                                                <asp:ListItem Value="PDF" Text="PDF (.pdf)"></asp:ListItem>
                                                <asp:ListItem Value="EXCEL" Text="Excel (.xlsx)"></asp:ListItem>
                                                <asp:ListItem Value="CSV" Text="CSV File (.csv)"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-6 form-group">
                                            <label class="form-label">Start Date</label>
                                            <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>
                                        <div class="col-6 form-group">
                                            <label class="form-label">End Date</label>
                                            <asp:TextBox runat="server" ID="txtEndDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
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
        </section>
    </div>
</asp:Content>