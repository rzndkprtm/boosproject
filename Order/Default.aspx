<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Order_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="List Order" %>

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

        <section class="row mb-3">
            <div class="col-12 d-flex justify-content-end flex-wrap gap-2">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Create Order" OnClick="btnAdd_Click" />
                <asp:Button runat="server" ID="btnAddMe" CssClass="btn btn-primary" Text="Create Order for Me" OnClick="btnAddMe_Click" />
                <asp:Button runat="server" ID="btnRework" CssClass="btn btn-info" Text="Rework Order" OnClick="btnRework_Click" />
                <asp:Button runat="server" ID="btnFile" CssClass="btn btn-secondary" Text="File Order" OnClick="btnFile_Click" />
            </div>
        </section>
        
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                    <div class="input-group">
                                        <asp:Label runat="server" CssClass="input-group-text" Text="Status"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlStatus" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-12 col-sm-12 col-lg-3 mb-2">
                                    <div class="input-group" runat="server" id="divCompany">
                                        <asp:Label runat="server" CssClass="input-group-text" Text="Company"></asp:Label>
                                        <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"></asp:DropDownList>
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
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                         <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                             <RowStyle />
                                             <Columns>
                                                 <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                     <ItemTemplate>
                                                         <%# Container.DataItemIndex + 1 %>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 <asp:BoundField DataField="Id" HeaderText="ID" />
                                                 <asp:BoundField DataField="OrderId" HeaderText="Order ID" />
                                                 <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" ItemStyle-Wrap="true" />
                                                 <asp:BoundField DataField="OrderNumber" HeaderText="Order Number" ItemStyle-Wrap="true" />
                                                 <asp:BoundField DataField="OrderName" HeaderText="Order Name" ItemStyle-Wrap="true" />
                                                 <asp:BoundField DataField="Status" HeaderText="Status" ItemStyle-Wrap="true" />
                                                 <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy}" />
                                                 <asp:BoundField DataField="SubmittedDate" HeaderText="Submitted" DataFormatString="{0:dd MMM yyyy}" />
                                                 <asp:BoundField DataField="ProductionDate" HeaderText="Production" DataFormatString="{0:dd MMM yyyy}" />
                                                 <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Shipment">
                                                     <ItemTemplate>
                                                         <a class="btn btn-sm btn-secondary" href="#" data-bs-toggle="modal" data-bs-target="#modalShipment" onclick='<%# String.Format("return showShipment(`{0}`, `{1:dd MMM yyyy}`, `{2}`, `{3}`);", Eval("ShipmentNumber").ToString(), Eval("ShipmentDate"), Eval("ContainerNumber").ToString(), Eval("Courier").ToString()) %>'>Show</a>
                                                     </ItemTemplate>
                                                 </asp:TemplateField>
                                                 <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                     <ItemTemplate>
                                                         <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                         <ul class="dropdown-menu">
                                                             <li>
                                                                 <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                             </li>
                                                             
                                                             <li runat="server" visible='<%# VisibleDelete(New Object() {Eval("Active"), Eval("Status"), Eval("CreatedBy"), Eval("CreatedRole")}) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "Delete Order") %>'>Delete</a>
                                                             </li>

                                                             <li runat="server" visible='<%# VisibleCopy(Eval("Active").ToString()) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "Copy Order") %>'>Copy / Duplicate</a>
                                                             </li>

                                                             <li runat="server" visible='<%# VisibleUnsubmitOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "Unsubmit Order") %>'>Unsubmit Order</a>
                                                             </li>

                                                             <li runat="server" visible='<%# VisibleProductionOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "Production Order") %>'>Production Order</a>
                                                             </li>
                                                             
                                                             <li runat="server" visible='<%# VisibleHoldOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "Hold Order") %>'>Hold Order</a>
                                                             </li>

                                                             <li runat="server" visible='<%# VisibleCancelOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalCancelOrder" onclick='<%# String.Format("return idCancelOrder(`{0}`);", Eval("Id").ToString()) %>'>Cancel Order</a>
                                                             </li>
                                                             
                                                             <li runat="server" visible='<%# VisibleRestore(Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalRestore" onclick='<%# String.Format("return showRestore(`{0}`);", Eval("Id").ToString()) %>'>Restore</a>
                                                             </li>

                                                             <li runat="server" visible='<%# VisibleShipmentOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalShipmentOrder" onclick='<%# String.Format("return idShipmentOrder(`{0}`);", Eval("Id").ToString()) %>'>Shipment Order</a>
                                                             </li>
                                                             
                                                             <li runat="server" visible='<%# VisibleReceivePayment(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "Receive Payment") %>'>Receive Payment</a>
                                                             </li>
                                                             
                                                             <li runat="server" visible='<%# VisibleCompleteOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "Complete Order") %>'>Complete Order</a>
                                                             </li>

                                                             <li runat="server" visible='<%# VisibleBOEOrder(Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalStatusOrder" onclick='<%# String.Format("return showStatusOrder(`{0}`, `{1}`);", Eval("Id").ToString(), "BOE Download") %>'>Authorize BOE Download</a>
                                                             </li>
                                                             
                                                             <li><hr class="dropdown-divider"></li>
                                                             
                                                             <li runat="server" visible='<%# VisiblePrintDO(Eval("CompanyId").ToString(), Eval("Status").ToString(), Eval("Active")) %>'>
                                                                 <a class="dropdown-item" href="#" data-bs-toggle="modal" data-bs-target="#modalPrintDO" onclick='<%# String.Format("return showPrintDO(`{0}`);", Eval("Id").ToString()) %>'>Print Delivery Order</a>
                                                             </li>

                                                             <li>
                                                                 <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('OrderHeaders', '<%# Eval("Id") %>')">Log</a>
                                                             </li>
                                                         </ul>
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
                    <div class="card-footer">
                        <div class="d-flex" runat="server" id="divActive">
                            <div class="ms-auto">
                                <div class="ms-2 d-inline-block">
                                    <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlActive_SelectedIndexChanged">
                                        <asp:ListItem Value="1" Text="Active"></asp:ListItem>
                                        <asp:ListItem Value="0" Text="Non Active"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal fade text-center" id="modalStatusOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white" id="titleStatus"></h5>
                </div>

                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdStatusOrder" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtStatusOrder" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnStatusOrder" CssClass="btn btn-info" Text="Confirm" OnClick="btnStatusOrder_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal fade text-left" id="modalCancelOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Cancel Order</h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdCancelOrder" style="display:none;"></asp:TextBox>
                    <div class="row mb-3">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtCancelDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divErrorCancelOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorCancelOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCancelOrder" CssClass="btn btn-danger" Text="Submit" OnClick="btnCancelOrder_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalShipmentOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Shipment Order</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdShipmentOrder" style="display:none;"></asp:TextBox>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Shipment Number</label>
                            <asp:TextBox runat="server" ID="txtShipmentNumber" CssClass="form-control" placeholder="Shipment Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Shipment Date</label>
                            <asp:TextBox runat="server" TextMode="Date" ID="txtShipmentDate" CssClass="form-control" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Container Number</label>
                            <asp:TextBox runat="server" ID="txtContainerNumber" CssClass="form-control" placeholder="Container Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Courier</label>
                            <asp:TextBox runat="server" ID="txtCourier" CssClass="form-control" placeholder="Courier ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
	
	                <div class="row mb-2" runat="server" id="divErrorShipmentOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorShipmentOrder"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnShipmentOrder" CssClass="btn btn-primary" Text="Submit" OnClick="btnShipmentOrder_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalRestore" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Restore Order</h5>
                </div>

                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdRestore" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitRestore" CssClass="btn btn-danger" Text="Confirm" OnClick="btnSubmitRestore_Click" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal fade text-center" id="modalPrintDO" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-secondary">
                    <h5 class="modal-title white">Print Delivery Order</h5>
                </div>

                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdPrintDO" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you want to print the delivery order for this order?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnPrintDO" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnPrintDO_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
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
        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) return;

            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';

                row.addEventListener('click', function (e) {
                    if (
                        e.target.closest("a") ||
                        e.target.closest("button") ||
                        e.target.closest("[data-bs-toggle]")
                    ) {
                        return;
                    }

                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                });
            }
        });

        function showShipment(number, date, container, courier) {
            document.getElementById("spanShipmentNumber").innerText = number;
            document.getElementById("spanShipmentDate").innerText = date;
            document.getElementById("spanContainerNumber").innerText = container;
            document.getElementById("spanCourier").innerText = courier;
        }

        function showStatusOrder(id, status) {
            document.getElementById("titleStatus").textContent = status;
            document.getElementById("<%=txtStatusOrder.ClientID %>").value = status;
            document.getElementById("<%=txtIdStatusOrder.ClientID %>").value = id;
        }

        function showCancelOrder() {
            $("#modalCancelOrder").modal("show");
        }

        function idShipmentOrder(id) {
            document.getElementById("<%=txtIdShipmentOrder.ClientID %>").value = id;
        }

        function showShipmentOrder() {
            $("#modalShipmentOrder").modal("show");
        }

        function showPrintDO(id) {
            document.getElementById("<%=txtIdPrintDO.ClientID %>").value = id;
        }

        function idCancelOrder(id) {
            document.getElementById("<%=txtIdCancelOrder.ClientID %>").value = id;
        }

        function showRestore(id) {
            document.getElementById("<%=txtIdRestore.ClientID %>").value = id;
        }

        function showLog(type, dataId) {
            $("#logError").addClass("d-none").html("");
            $("#tblLogs tbody").html("");
            $("#modalLog").modal("show");

            $.ajax({
                type: "POST",
                url: "Method.aspx/GetLogs",
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

        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 5000);

            return true;
        }

        ["modalShipment", "modalStatusOrder", "modalCancelOrder", "modalShipmentOrder", "modalRestore", "modalPrintDO", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
