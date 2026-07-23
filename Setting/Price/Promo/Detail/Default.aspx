<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Price_Promo_Detail_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Price Promo Detail" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/promo">Promo</a></li>
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
            <div class="col-12">
                <div class="card shadow-sm border-0">
                    <div class="card-header bg-light">
                        <div class="d-flex justify-content-between align-items-center">
                            <h5 class="card-title mb-0">Promo Information</h5>
                            <div>
                                <asp:Button runat="server" ID="btnEdit" CssClass="btn btn-primary btn-sm" Text="Edit Promo" OnClick="btnEdit_Click" />
                                <a href="javascript:void(0);" runat="server" id="aDelete" class="btn btn-outline-danger btn-sm" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                                <a href="javascript:void(0);" class="btn btn-outline-secondary btn-sm" onclick="showLog('Promos', '<%= lblId.Text %>')">Log</a>
                            </div>
                        </div>
                    </div>
                    <div class="card-body">
                        <div class="row mt-4">
                            <div class="col-md-4 mb-3">
                                <label class="text-muted small">Company</label>
                                <div class="fw-bold">
                                    <asp:Label runat="server" ID="lblCompanyDetail"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label class="text-muted small">Promo Name</label>
                                <div class="fw-bold">
                                    <asp:Label runat="server" ID="lblName"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-4 mb-3">
                                <label class="text-muted small">Status</label>
                                <div>
                                    <asp:Label runat="server" ID="lblActive"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-3 mb-3">
                                <label class="text-muted small">Start Date</label>
                                <div>
                                    <asp:Label runat="server" ID="lblStartDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-3 mb-3">
                                <label class="text-muted small">End Date</label>
                                <div>
                                    <asp:Label runat="server" ID="lblEndDate"></asp:Label>
                                </div>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label class="text-muted small">Description</label>
                                <div>
                                    <asp:Label runat="server" ID="lblDescription"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card shadow-sm">
                    <div class="card-header bg-light">
                        <div class="d-flex justify-content-between align-items-center">
                            <h3 class="card-title">Detail Promo</h3>
                            <asp:Button runat="server" ID="btnAddDetail" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddDetail_Click" />
                        </div>
                    </div>
                    <div class="card-body mt-4 p-0">
                        <div class="table-responsive">
                            <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="Id" HeaderText="ID" />
                                    <asp:BoundField DataField="Type" HeaderText="Type" />
                                    <asp:TemplateField HeaderText="Discount Tile">
                                        <ItemTemplate>
                                            <%# DiscountTitle(Eval("Type").ToString(), Eval("DataId").ToString()) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Discount">
                                        <ItemTemplate>
                                            <%# DiscountValue(Eval("Discount")) %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="130px">
                                        <ItemTemplate>
                                            <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                            <ul class="dropdown-menu">
                                                <li runat="server" visible='<%# LoginAccess("Change Detail") %>'>
                                                    <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalChangeDetail" onclick='<%# String.Format("return dataChangeDetail(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Discount").ToString()) %>'>Change Value</a>
                                                </li>
                                                <li runat="server" visible='<%# LoginAccess("Delete Detail") %>'>
                                                    <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteDetail" onclick='<%# String.Format("return showDeleteDetail(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                </li>
                                                <li>
                                                    <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('PromoDetails', '<%# Eval("Id") %>')">Log</a>
                                                </li>
                                            </ul>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="card-footer"></div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Promo</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtDeleteId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDelete_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalChangeDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Change Value</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtChangeValueId" style="display:none;"></asp:TextBox>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Discount</label>
                            <asp:TextBox runat="server" ID="txtDiscount" CssClass="form-control" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnChangeDetail" CssClass="btn btn-info" Text="Submit" OnClick="btnChangeDetail_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Promo Detail</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdDeleteDetail" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteDetail" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteDetail_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Changelog</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger d-none" id="logError"></div>
                    <div class="table-responsive">
                        <table class="table table-vcenter card-table" id="tblLogs">
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        function dataChangeDetail(id, discount) {
            document.getElementById("<%=txtChangeValueId.ClientID %>").value = id;
            document.getElementById("<%=txtDiscount.ClientID %>").value = discount;
        }
        function showLog(type, dataId) {
            $("#logError").addClass("d-none").html("");
            $("#tblLogs tbody").html("");
            $("#modalLog").modal("show");

            $.ajax({
                type: "POST",
                url: "/Setting/Method.aspx/GetLogs",
                data: JSON.stringify({ type: type, dataId: dataId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    const logs = res.d;

                    if (!logs || logs.length === 0) {
                        $("#tblLogs tbody").html(
                            `<tr><td class="text-center">DATA LOG NOT FOUND</td></tr>`
                        );
                        return;
                    }

                    let html = "";
                    logs.forEach(r => {
                        html += `<tr><td>${r.TextLog}</td></tr>`;
                    });

                    $("#tblLogs tbody").html(html);
                },
                error: function (err) {
                    $("#logError").removeClass("d-none").html("FAILED TO LOAD LOG DATA");
                }
            });
        }
        ["modalDelete", "modalChangeDetail", "modalDeleteDetail", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) return;
            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';
                row.addEventListener('click', function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                        return;
                    }
                    const btn = this.querySelector("a[id*='aEditDetailPromo']");
                    if (btn) btn.click();
                });
            }
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
