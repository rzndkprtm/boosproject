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
                            <li class="breadcrumb-item"><a href='<%= ResolveUrl("~/order/detail?orderid=" & lblHeaderId.Text) %>'>Detail</a></li>
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
                                        <asp:Label runat="server" ID="lblOrderId" CssClass="form-control font-bold"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Order Number</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblOrderNumber" CssClass="form-control font-bold"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Order Name</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblOrderName" CssClass="form-control font-bold"></asp:Label>
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
                                    <div class="row mt-4 mb-3">
                                        <div class="col-12">
                                            <span runat="server" class="font-extrabold" id="spanFabric"></span>
                                        </div>
                                    </div>
                                    <div class="row mb-3 gallery">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrinting" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12">
                                            <a href="javascript:void(0)" runat="server" id="aUpload" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalUpload">Upload Image</a>
                                            <a href="javascript:void(0)" runat="server" id="aDelete" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete Image</a>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" runat="server" id="divPrintingB" ClientIDMode="Static" role="tabpanel" aria-labelledby="aPrintingB">
                                    <div class="row mt-4 mb-3">
                                        <div class="col-12">
                                            <span runat="server" class="font-extrabold" id="spanFabricB"></span>
                                        </div>
                                    </div>
                                    <div class="row mb-3 gallery">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrintingB" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12">
                                            <a href="javascript:void(0)" runat="server" id="aUploadB" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalUploadB">Upload Image</a>
                                            <a href="javascript:void(0)" runat="server" id="aDeleteB" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDeleteB">Delete Image</a>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" runat="server" id="divPrintingC" ClientIDMode="Static" role="tabpanel" aria-labelledby="aPrintingC">
                                    <div class="row mt-4 mb-3">
                                        <div class="col-12">
                                            <span runat="server" class="font-extrabold" id="spanFabricC"></span>
                                        </div>
                                    </div>
                                    <div class="row mb-3 gallery">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrintingC" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12">
                                            <a href="javascript:void(0)" runat="server" id="aUploadC" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalUploadC">Upload Image</a>
                                            <a href="javascript:void(0)" runat="server" id="aDeleteC" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDeleteC">Delete Image</a>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" runat="server" id="divPrintingD" ClientIDMode="Static" role="tabpanel" aria-labelledby="aPrintingD">
                                    <div class="row mt-4 mb-3">
                                        <div class="col-12">
                                            <span runat="server" class="font-extrabold" id="spanFabricD"></span>
                                        </div>
                                    </div>
                                    <div class="row mb-3 gallery">
                                        <div class="col-12">
                                            <asp:Image runat="server" CssClass="w-100" ID="imgPrintingD" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12">
                                            <a href="javascript:void(0)" runat="server" id="aUploadD" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalUploadD">Upload Image</a>
                                            <a href="javascript:void(0)" runat="server" id="aDeleteD" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDeleteD">Delete Image</a>
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

    <div class="modal fade" id="modalUpload" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Upload File</h5>
                </div>
                <div class="modal-body">
                    <label class="form-label">Upload New Image</label>
                    <asp:FileUpload runat="server" ID="fuUpload" CssClass="form-control" />
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUpload" CssClass="btn btn-info" Text="Submit" OnClick="btnUpload_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalUploadB" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Upload File</h5>
                </div>
                <div class="modal-body">
                    <label class="form-label">Upload New Image</label>
                    <asp:FileUpload runat="server" ID="fuUploadB" CssClass="form-control" />
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUploadB" CssClass="btn btn-info" Text="Submit" OnClick="btnUploadB_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalUploadC" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Upload File</h5>
                </div>
                <div class="modal-body">
                    <label class="form-label">Upload New Image</label>
                    <asp:FileUpload runat="server" ID="fuUploadC" CssClass="form-control" />
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUploadC" CssClass="btn btn-info" Text="Submit" OnClick="btnUploadC_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalUploadD" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Upload File</h5>
                </div>
                <div class="modal-body">
                    <label class="form-label">Upload New Image</label>
                    <asp:FileUpload runat="server" ID="fuUploadD" CssClass="form-control" />
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnUploadD" CssClass="btn btn-info" Text="Submit" OnClick="btnUploadD_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Image</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDelete_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDeleteB" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Image</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteB" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteB_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDeleteC" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Image</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteC" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteC_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDeleteD" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Image</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteD" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteD_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField ID="selected_tab" runat="server" />

    <script type="text/javascript">
        [
            "modalUpload", "modalUploadB", "modalUploadC", "modalUploadD",
            "modalDelete", "modalDeleteB", "modalDeleteC", "modalDeleteD"
        ].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblHeaderId"></asp:Label>
        <asp:Label runat="server" ID="lblItemId"></asp:Label>
        <asp:Label runat="server" ID="lblStatus"></asp:Label>

        <asp:Label runat="server" ID="lblPrinting"></asp:Label>
        <asp:Label runat="server" ID="lblPrintingB"></asp:Label>
        <asp:Label runat="server" ID="lblPrintingC"></asp:Label>
        <asp:Label runat="server" ID="lblPrintingD"></asp:Label>
        <asp:Label runat="server" ID="lblPrintingE"></asp:Label>
        <asp:Label runat="server" ID="lblPrintingF"></asp:Label>
    </div>
</asp:Content>