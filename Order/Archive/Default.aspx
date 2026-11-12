<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Order_Archive_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="List Order (Archive)" %>

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
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
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
            <div class="col-12">
                <asp:UpdatePanel runat="server" ID="upList" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                        <div class="input-group">
                                            <asp:Label runat="server" CssClass="input-group-text" Text="Status"></asp:Label>
                                            <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged">
                                                <asp:ListItem Value="" Text="All Orders"></asp:ListItem>
                                                <asp:ListItem Value="Draft" Text="Draft"></asp:ListItem>
                                                <asp:ListItem Value="Quoted" Text="Quoted"></asp:ListItem>
                                                <asp:ListItem Value="New Order" Text="New Order"></asp:ListItem>
                                                <asp:ListItem Value="Waiting Proforma" Text="Waiting Proforma"></asp:ListItem>
                                                <asp:ListItem Value="Proforma Sent" Text="Proforma Sent"></asp:ListItem>
                                                <asp:ListItem Value="Payment Received" Text="Payment Received"></asp:ListItem>
                                                <asp:ListItem Value="In Production" Text="In Production"></asp:ListItem>
                                                <asp:ListItem Value="On Hold" Text="On Hold"></asp:ListItem>
                                                <asp:ListItem Value="Shipped Out" Text="Shipped Out"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                        <div class="input-group" runat="server" id="divActive">
                                            <asp:Label runat="server" CssClass="input-group-text" Text="Status"></asp:Label>
                                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlActive_SelectedIndexChanged">
                                                <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                                <asp:ListItem Value="0" Text="Non Active"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                        <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                            <div class="input-group">
                                                <span class="input-group-text">Search</span>
                                                <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder="Order ID, Customer Name, Order Number, Order Name ....." autocomplete="off"></asp:TextBox>
                                                <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Visible="false" OnPageIndexChanging="gvList_PageIndexChanging" OnDataBound="gvList_DataBound">
                                        <RowStyle />
                                        <Columns>
                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="OrdID" HeaderText="ID" />
                                            <asp:BoundField DataField="StoreName" HeaderText="Customer Name" ItemStyle-Wrap="true" />
                                            <asp:BoundField DataField="StoreOrderNo" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                            <asp:BoundField DataField="StoreCustomer" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                            <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-Wrap="true" />
                                            <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy}" />
                                            <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Shipment">
                                                <ItemTemplate>
                                                    <a class="btn btn-sm btn-secondary" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalShipment" onclick='<%# String.Format("return showShipment(`{0}`, `{1:dd MMM yyyy}`, `{2}`, `{3}`);", Eval("ShipmentNo").ToString(), Eval("Shipped"), Eval("ConNote").ToString(), Eval("Courier").ToString()) %>'>Show</a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <a class="btn btn-sm btn-primary" id="aDetail" href='<%# Page.ResolveUrl("~/order/archive/detail?orderid=" & Eval("OrdID").ToString()) %>'>Detail</a>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <div class="d-flex justify-content-end mt-3">
                                    <nav runat="server" id="navPager" visible="false">
                                        <ul class="pagination mb-0">
                                            <asp:Repeater runat="server" ID="rptPager" OnItemCommand="rptPager_ItemCommand">
                                                <ItemTemplate>
                                                    <li class='page-item <%# Eval("CssClass") %>'>
                                                        <asp:LinkButton runat="server" ID="lnkPage" CssClass="page-link" Text='<%# Eval("Text") %>' CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>' />
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>
                                    </nav>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </section>
    </div>
    <div class="modal fade text-center" id="modalShipment" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Detail Shipment</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover">
                            <tr>
                                <th>Shipment Number</th>
                                <th>Shipment Date</th>
                                <th>Container Number</th>
                                <th>Courier</th>
                            </tr>
                            <tr>
                                <td><span id="spanShipmentNumber"></span></td>
                                <td><span id="spanShipmentDate"></span></td>
                                <td><span id="spanContainerNumber"></span></td>
                                <td><span id="spanCourier"></span></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                </div>
            </div>
        </div>
    </div>
    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) {
                loading.style.display = "none";
            }
        });
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") {
                return;
            }
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) {
                    loading.style.display = "block";
                }
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) {
                    loading.style.display = "none";
                }
                bindGridRowClick();
            });
        }
        function bindGridRowClick() {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) {
                return;
            }
            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = "pointer";
                row.onclick = function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]") || e.target.closest(".dropdown-menu")) {
                        return;
                    }
                    const btn = this.querySelector("a[id*='aDetail']");
                    if (btn) {
                        btn.click();
                    }
                };
            }
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            bindGridRowClick();
        });
        function showShipment(number, date, container, courier) {
            document.getElementById("spanShipmentNumber").innerText = number;
            document.getElementById("spanShipmentDate").innerText = date;
            document.getElementById("spanContainerNumber").innerText = container;
            document.getElementById("spanCourier").innerText = courier;
        }
        ["modalShipment"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>