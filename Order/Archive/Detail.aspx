<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Order_Archive_Detail" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Order Detail (Archive)" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">
                        This list contains orders from the previous system.
                        <br />
                        If any order needs reprocessing, please contact customer service or submit a support ticket so we can move it back to the main order queue.
                    </p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/order">Order</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/order/archive">Order Archive</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row mb-3">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <asp:Button runat="server" ID="btnPreview" CssClass="btn btn-primary" Text="Preview"  OnClick="btnPreview_Click" />
                <a class="btn btn-secondary" href="#" runat="server" id="aConvert" data-bs-toggle="modal" data-bs-target="#modalConvert">Unarchive Order</a>
            </div>
        </section>

        <section class="row mb-3">
            <div class="col-12">
                <div class="row mb-2" runat="server" id="divError">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section class="row">
            <div class="col-12 col-sm-12 col-lg-7">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-9">
                                    <label>Customer Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCustomerName" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-6 mb-3">
                                    <label>Store Order Number</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderNumber" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-6 mb-3">
                                    <label>Store Customer</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOrderName" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-12 col-sm-12 col-lg-5">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-6">
                                    <label>Created By</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCreatedBy" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-6">
                                    <label>Created Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCreatedDate" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-6">
                                    <label>Submitted By</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblSubmittedBy" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-6">
                                    <label>Submitted Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblSubmittedDate" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section class="row mb-3">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <h3 class="card-title">Your Item</h3>
                        </div>

                        <div class="card-body">
                            <div class="row">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvListItem" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="OrddID" HeaderText="ID" />
                                                <asp:BoundField DataField="Room" HeaderText="Room" />
                                                <asp:TemplateField HeaderText="Description">
                                                    <ItemTemplate>
                                                        <%# BindProductDescription(Eval("OrddID").ToString()) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Width" HeaderText="Width" />
                                                <asp:BoundField DataField="Drop" HeaderText="Drop" />
                                                <asp:BoundField DataField="Fabric" HeaderText="Fabric" />
                                                <asp:TemplateField HeaderText="Price">
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                            <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalConvert" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Unarchive Order</h5>
                </div>

                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnConvert" CssClass="btn btn-info" Text="Confirm" OnClick="btnConvert_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalWaiting" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-body text-center py-4">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 30000);

            return true;
        }

        ["modalConvert"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
    </script>
</asp:Content>