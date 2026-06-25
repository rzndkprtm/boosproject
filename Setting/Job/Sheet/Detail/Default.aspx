<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Job_Sheet_Detail_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Job Sheet Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .info-box { padding: 12px 14px; border-radius: 10px; background: #f8f9fa; border: 1px solid #eee; }
        .info-box label { font-size: 12px; letter-spacing: .3px; text-transform: uppercase; }
        .info-box .fw-semibold { font-size: 15px; }
        .card { transition: all .2s ease-in-out; }
        .card:hover { transform: translateY(-2px); box-shadow: 0 8px 25px rgba(0,0,0,0.08); }
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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/job">Job</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/job/sheet">Sheet</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row mb-2">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <asp:Button runat="server" ID="btnEdit" CssClass="btn btn-info" Text="Edit" OnClick="btnEdit_Click" />
                <a href="javascript:void(0);" class="btn btn-secondary" onclick="showLog('JobSheets', '<%= lblId.Text %>')">Log</a>
            </div>
        </section>
        <section class="row justify-content-center">
            <div class="col-12">
                <div class="card shadow-sm border-0 rounded-3">
                    <div class="card-header bg-white border-bottom-0 pt-4 pb-0">
                        <h5 class="mb-0 fw-semibold text-dark">Job Sheet Data</h5>
                    </div>
                    <div class="card-body pt-4">
                        <div class="row g-4">
                            <div class="col-12 col-md-4">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Sheet Name</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblName"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-4">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Alias</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblAlias"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-4">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Active Status</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblActive" CssClass="badge rounded-pill bg-success text-white"></asp:Label>
                                    </div>
                                </div>
                            </div>
                            <div class="col-12 col-md-6">
                                <div class="info-box">
                                    <label class="form-label text-muted mb-1">Description</label>
                                    <div class="fw-semibold fs-6 text-dark">
                                        <asp:Label runat="server" ID="lblDescription"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="row" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-6">
                                    <h3 class="card-title">List Formula</h3>
                                </div>
                                <div class="col-6 d-flex justify-content-end">
                                    <asp:Button runat="server" ID="btnAddDetail" CssClass="btn btn-primary me-1" Text="Add New" OnClick="btnAddDetail_Click" />
                                    <a class="btn btn-dark" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalSortOrder"'>Change Sort Order</a>
                                </div>
                            </div>
                        </div>
                        <div class="card-body">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvListDetail" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                        <asp:BoundField DataField="Formula1" HeaderText="Formula 1" />
                                        <asp:BoundField DataField="SortOrder" HeaderText="Sort Order" />
                                        <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="130px">
                                            <ItemTemplate>
                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                <ul class="dropdown-menu">
                                                    <li>
                                                        <a class="dropdown-item" runat="server" id="aFormulaDetail" href="javascript:void(0);" data-bs-toggle="modal" data-bs-target="#modalFormula" onclick='<%# String.Format("return dataFormula(`{0}`, `{1}`, `{2}`, `{3}`, `{4}`, `{5}`, `{6}`, `{7}`);", Eval("Formula1").ToString(), Eval("Formula2"), Eval("Formula3").ToString(), Eval("Formula4"), Eval("Formula5").ToString(), Eval("Formula6").ToString(), Eval("Formula7").ToString(), Eval("Formula8").ToString()) %>'>All Formula</a>
                                                    </li>
                                                    <li>
                                                        <a class="dropdown-item" runat="server" id="aEditDetail" href='<%# Page.ResolveUrl("~/setting/job/sheet/detail/edit?detailid=" & Eval("Id")) %>'>Edit</a>
                                                    </li>
                                                    <li>
                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteDetail" onclick='<%# String.Format("return dataDeleteDetail(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                    </li>
                                                    <li>
                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('JobSheetDetails', '<%# Eval("Id") %>')">Log</a>
                                                    </li>
                                                </ul>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalSortOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Sort Order</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListSortOrder" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeaderWhenEmpty="true" DataKeyNames="Id">
                            <Columns>
                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                <asp:BoundField DataField="SortOrder" HeaderText="Sort Order (Current)" />
                                <asp:TemplateField HeaderText="Sort Order (New)">
                                    <ItemTemplate>
                                        <asp:TextBox runat="server" ID="txtSortOrder" CssClass="form-control" Text='<%# Eval("SortOrder").ToString() %>'></asp:TextBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                    <asp:Button runat="server" ID="btnSortOrder" CssClass="btn btn-dark" Text="Submit" OnClick="btnSortOrder_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalFormula" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-full modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">All Formula</h5>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover">
                            <tr>
                                <th>Formula 1</th>
                                <th>Formula 2</th>
                                <th>Formula 3</th>
                                <th>Formula 4</th>
                            </tr>
                            <tr>
                                <td><span id="spanFormula1"></span></td>
                                <td><span id="spanFormula2"></span></td>
                                <td><span id="spanFormula3"></span></td>
                                <td><span id="spanFormula4"></span></td>
                            </tr>
                        </table>
                    </div>
                    <div class="table-responsive">
                        <table class="table table-bordered table-hover">
                            <tr>
                                <th>Formula 5</th>
                                <th>Formula 6</th>
                                <th>Formula 7</th>
                                <th>Formula 8</th>
                            </tr>
                            <tr>
                                <td><span id="spanFormula5"></span></td>
                                <td><span id="spanFormula6"></span></td>
                                <td><span id="spanFormula7"></span></td>
                                <td><span id="spanFormula8"></span></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Job Sheet Detail</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtDeleteDetailId" style="display:none;"></asp:TextBox>
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
        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvListDetail.ClientID %>');
            if (!gv) return;
            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';
                row.addEventListener('click', function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                        return;
                    }
                    const btn = this.querySelector("a[id*='aFormulaDetail']");
                    if (btn) btn.click();
                });
            }
        });
        function dataFormula(formula1, formula2, formula3, formula4, formula5, formula6, formula7, formula8) {
            document.getElementById("spanFormula1").innerText = formula1;
            document.getElementById("spanFormula2").innerText = formula2;
            document.getElementById("spanFormula3").innerText = formula3;
            document.getElementById("spanFormula4").innerText = formula4;
            document.getElementById("spanFormula5").innerText = formula5;
            document.getElementById("spanFormula6").innerText = formula6;
            document.getElementById("spanFormula7").innerText = formula7;
            document.getElementById("spanFormula8").innerText = formula8;
        }
        function dataDeleteDetail(id) {
            document.getElementById("<%=txtDeleteDetailId.ClientID %>").value = id;
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
        ["modalSortOrder", "modalFormula", "modalDeleteDetail", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>