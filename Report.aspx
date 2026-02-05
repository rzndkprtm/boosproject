<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Report.aspx.vb" Inherits="Report" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Report" %>

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
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row mb-3">
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Create Report</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="form-body">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label class="form-label">Company</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label class="form-label">Sub Company</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-5 form-group">
                                            <asp:DropDownList runat="server" ID="ddlSubCompany" CssClass="form-select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label class="form-label">Report Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                                <asp:ListItem Value="Blind Orders" Text="Blind Orders"></asp:ListItem>
                                                <asp:ListItem Value="Shutter Orders" Text="Shutter Orders"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label class="form-label">File Type</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <asp:DropDownList runat="server" ID="ddlFile" CssClass="form-select">
                                                <asp:ListItem Value="PDF" Text="PDF"></asp:ListItem>
                                                <asp:ListItem Value="Screen" Text="Screen"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-3">
                                            <label class="form-label">Date</label>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                                            <asp:TextBox runat="server" ID="txtEndDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
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

        <section class="row" runat="server" id="secScreen">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Report Result</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="col-12">
                                <div class="table-responsive">
                                    <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" EmptyDataText="DATA NOT FOUND :)" Width="150%" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCreated="gvList_RowCreated"></asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>
</asp:Content>