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
        <section class="row mb-4">
            <div class="col-12 d-flex justify-content-end flex-wrap gap-2">
                <asp:Button runat="server" ID="btnGenerate" CssClass="btn btn-success" Text="Generate Report" OnClick="btnGenerate_Click" />
            </div>
        </section>
        <section class="row mb-3" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12 col-sm-12 col-lg-4">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Report Filters</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Company</label>
                                        <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select"></asp:DropDownList>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group" runat="server" id="divCompany">
                                        <label class="form-label">Status</label>
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select">
                                            <asp:ListItem Value="" Text=""></asp:ListItem>
                                            <asp:ListItem Value="Submitted" Text="Submitted"></asp:ListItem>
                                            <asp:ListItem Value="In Production" Text="In Production"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">Start Date</label>
                                        <asp:TextBox runat="server" ID="txtStartDate" CssClass="form-control" TextMode="Date"></asp:TextBox>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 form-group">
                                        <label class="form-label">End Date</label>
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
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Report Output</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" role="tablist">
                                <a class="list-group-item list-group-item-action active" id="listCustomer" data-bs-toggle="list" href="#list-customer" role="tab">Customers</a>
                                <a class="list-group-item list-group-item-action" id="listProduct" data-bs-toggle="list" href="#list-product" role="tab">Products</a>
                                <a class="list-group-item list-group-item-action" id="listFabric" data-bs-toggle="list" href="#list-fabric" role="tab">Fabrics</a>
                                <a class="list-group-item list-group-item-action" id="listBottom" data-bs-toggle="list" href="#list-bottom" role="tab">Bottom Rails</a>

                                <a class="list-group-item list-group-item-action" id="listTube" data-bs-toggle="list" href="#list-tube" role="tab">Tube Types</a>
                            </div>
                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-customer" role="tabpanel" aria-labelledby="listCustomer">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="grid-container table-responsive">
                                                <asp:GridView runat="server" ID="gvBlindsPivot" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvBlindsPivot_RowDataBound"></asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane fade" id="list-product" role="tabpanel" aria-labelledby="listProduct">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvList_RowDataBound"></asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane fade" id="list-fabric" role="tabpanel" aria-labelledby="listFabric">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvFabric" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center"></asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane fade" id="list-bottom" role="tabpanel" aria-labelledby="listBottom">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvBottom" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center"></asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane fade" id="list-tube" role="tabpanel" aria-labelledby="listTube">
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvTube" CssClass="table table-bordered table-hover" AutoGenerateColumns="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center"></asp:GridView>
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