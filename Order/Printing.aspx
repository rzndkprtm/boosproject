<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Printing.aspx.vb" Inherits="Order_Printing" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Printing Fabric" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/order/detail">Detail</a></li>
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
            <div class="col-12 col-sm-12 col-lg-3">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title">Order Data</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Order #</label>
                                        <br />
                                        <label runat="server" id="lblOrderId" class="font-bold">Order #</label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Order Number</label>
                                        <br />
                                        <label runat="server" id="lblOrderNumber" class="font-bold">Order Number</label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Order Name</label>
                                        <br />
                                        <label runat="server" id="lblOrderName" class="font-bold">Order Name</label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-9">
                <div class="card">
                    <div class="card-header">
                        <h4 class="card-title" runat="server" id="descProduct">Order Data</h4>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" role="tablist">
                                <a class="list-group-item list-group-item-action" runat="server" id="aPrinting" data-bs-toggle="list" href="#divPrinting" role="tab">First Fabric</a>
                                <a class="list-group-item list-group-item-action" runat="server" id="aPrintingB" data-bs-toggle="list" href="#divPrintingB" role="tab">Second Fabric</a>
                                <a class="list-group-item list-group-item-action" runat="server" id="aPrintingC" data-bs-toggle="list" href="#divPrintingC" role="tab">Thrid Fabric</a>
                                <a class="list-group-item list-group-item-action" runat="server" id="aPrintingD" data-bs-toggle="list" href="#divPrintingD" role="tab">Fourth Fabric</a>
                            </div>

                            <div class="tab-content text-justify">
                                <div runat="server" class="tab-pane fade" id="divPrinting" ClientIDMode="Static" role="tabpanel" aria-labelledby="aPrinting">
                                    <div class="row gallery mt-4">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrinting" />
                                            <asp:Button runat="server" ID="btnDeletePrinting" CssClass="btn btn-danger mt-2" Text="Delete" />
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" runat="server" id="divPrintingB" ClientIDMode="Static" role="tabpanel" aria-labelledby="aPrintingB">
                                    <div class="row gallery mt-4">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrintingB" />
                                            <asp:Button runat="server" ID="btnDeletePrintingB" CssClass="btn btn-danger mt-2" Text="Delete" />
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" runat="server" id="divPrintingC" ClientIDMode="Static" role="tabpanel" aria-labelledby="aPrintingC">
                                    <div class="row gallery mt-4">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrintingC" />
                                            <asp:Button runat="server" ID="btnDeletePrintingC" CssClass="btn btn-danger mt-2" Text="Delete" />
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" runat="server" id="divPrintingD" ClientIDMode="Static" role="tabpanel" aria-labelledby="aPrintingD">
                                    <div class="row gallery mt-4">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrintingD" />
                                            <asp:Button runat="server" ID="btnDeletePrintingD" CssClass="btn btn-danger mt-2" Text="Delete" />
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
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
    </div>
</asp:Content>