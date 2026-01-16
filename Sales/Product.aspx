<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Product.aspx.vb" Inherits="Sales_Product" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Sales Product" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Timer runat="server" ID="tmrTicket" Interval="5000" OnTick="tmrTicket_Tick" />
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted">Displaying the total number of ordered items for each product from every area that entered production today.</p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/sales">Sales</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row mb-2" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-header">
                        <div class="row">
                            <div class="col-7">
                                <h3 class="card-title">Date : <%= DateTime.Now.ToString("dd MMM yyyy") %></h3>
                            </div>
                            <div class="col-5 d-flex justify-content-end">
                                <a href="#" class="btn btn-success" data-bs-toggle="modal" data-bs-target="#modalCustom">Custom Data</a>
                            </div>
                        </div>
                    </div>
                    <div class="card-content">
                        <div class="card-body">
                            <asp:UpdatePanel runat="server" ID="upDetail" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <div class="row">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover auto-width-grid" AutoGenerateColumns="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCreated="gvList_RowCreated"></asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="tmrTicket" EventName="Tick" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="card-footer text-center"></div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalCustom" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Custom Data</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Area</label>
                            <asp:ListBox runat="server" ID="lbArea" CssClass="choices form-select multiple-remove" SelectionMode="Multiple">
                                <asp:ListItem Value="NSW 1" Text="NSW 1"></asp:ListItem>
                                <asp:ListItem Value="NSW 2" Text="NSW 2"></asp:ListItem>
                                <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                                <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                                <asp:ListItem Value="VIC 1" Text="VIC 1"></asp:ListItem>
                                <asp:ListItem Value="VIC 2" Text="VIC 2"></asp:ListItem>
                                <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
                            </asp:ListBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Product</label>
                            <asp:ListBox runat="server" ID="lbDesign" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                        </div>
                    </div>
		
		            <div class="row mb-2" runat="server" id="divErrorCustom">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorCustom"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCustom" CssClass="btn btn-primary" Text="Submit" OnClick="btnCustom_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
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
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            document.title = '<%= Page.Title %> - <%= Session("CompanyName") %>';
        });

        ["modalCustom", "modalWaiting"].forEach(id => {
            document.getElementById(id).addEventListener("hide.bs.modal", () => {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        function showCustom() {
            $("#modalCustom").modal("show");
        }

        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 10000);

            return true;
        }

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>