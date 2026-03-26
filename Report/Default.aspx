<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Report_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Report" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .grid-container { width: 100%; height: calc(100vh - 150px); overflow: auto; border: 1px solid #ddd; }
        .grid-container table { width: 100%; border-collapse: collapse; table-layout: auto; }
        .grid-container td, .grid-container th { white-space: nowrap; padding: 6px 10px; }
        .grid-container th { position: sticky; top: 0; background: #f5f5f5; z-index: 3; }
        .grid-container td:first-child, .grid-container th:first-child { position: sticky; left: 0; background: #fff; z-index: 2; white-space: normal; word-break: break-word; min-width: 220px; max-width: 320px; }
        .grid-container th:first-child { z-index: 5; background: #f5f5f5; }
        .grid-container tr:last-child td { position: sticky; bottom: 0; background: #f5f5f5; z-index: 3; }
        .grid-container tr:last-child td:first-child { left: 0; z-index: 6; background: #f5f5f5; }
    </style>
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
        <section class="row mb-3" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>

        <section class="row">
            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Report Filters</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-horizontal">
                                <div class="row" runat="server" id="divCompany">
                                    <div class="col-12 col-sm-12 col-lg-3">
                                        <label class="form-label">Company</label>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-3">
                                        <label class="form-label">Status</label>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Submitted" Text="Submitted"></asp:ListItem>
                                            <asp:ListItem Value="In Production" Text="In Production"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-3">
                                        <label class="form-label">Date</label>
                                    </div>
                                    <div class="col-6 col-sm-6 col-lg-4 form-group">
                                        <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-6 col-sm-6 col-lg-4 form-group">
                                        <asp:TextBox runat="server" ID="txtEndDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
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

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Report Output</h4>
                    </div>

                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" role="tablist">
                                <a class="list-group-item list-group-item-action active" id="list-sunday-list" data-bs-toggle="list" href="#list-sunday" role="tab">Output</a>
                                <a class="list-group-item list-group-item-action" id="list-monday-list" data-bs-toggle="list" href="#list-monday" role="tab">Output Include Customer</a>
                            </div>

                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-sunday" role="tabpanel" aria-labelledby="list-sunday-list">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvList_RowDataBound"></asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-monday" role="tabpanel" aria-labelledby="list-monday-list">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="grid-container table-responsive">
                                                <asp:GridView runat="server" ID="gvBlindsPivot" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvBlindsPivot_RowDataBound"></asp:GridView>
                                            </div>
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

    <script type="text/javascript">
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>