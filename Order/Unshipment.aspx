<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Unshipment.aspx.vb" Inherits="Order_Unshipment" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Unshipment Order" %>

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
        <section class="row mb-4">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <button class="btn btn-primary dropdown-toggle me-1" type="button" data-bs-toggle="dropdown" aria-expanded="false">Generate PDF</button>
                <ul class="dropdown-menu">
                    <li><asp:Button runat="server" ID="btnPreview" CssClass="dropdown-item" Text="Preview" /></li>
                    <li><asp:Button runat="server" ID="btnDownload" CssClass="dropdown-item" Text="Download" OnClick="btnDownload_Click" /></li>
                </ul>
                <a href="#" runat="server" id="aEmail" class="btn btn-danger me-1" data-bs-toggle="modal" data-bs-target="#modalSendEmail">Send Via Email</a>
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                    <h4 class="card-title">List Order</h4>
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
                            <div class="row">
                                <div class="col-12">
                                    <div class="alert alert-light-warning color-warning">
                                        <i class="bi bi-exclamation-circle"></i>
                                        The displayed data is retrieved from orders that have entered production (based on the production date) and have not been shipped for more than 10 days.
                                    </div>
                                </div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="100" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
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
                                                <asp:BoundField DataField="ProductionDate" HeaderText="Production" DataFormatString="{0:dd MMM yyyy}" />
                                                <asp:TemplateField HeaderText="Factory">
                                                    <ItemTemplate>
                                                        <%# BindFactory(Eval("Id")) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
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
                        <div class="d-flex" runat="server" id="divCompany">
                            <div class="ms-auto">
                                <div class="ms-2 d-inline-block">
                                    <div class="input-group">
                                        <span class="input-group-text">Company</span>
                                        <asp:DropDownList runat="server" ID="ddlCompany" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalSendEmail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Send Email</h4>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">TO</label>
                            <asp:TextBox runat="server" ID="txtSendEmailTo" TextMode="MultiLine" CssClass="form-control" Height="135px" placeholder="CC Customer ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            <p><small class="text-muted">* Please enter the required email address in a new line.</small></p>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">CC</label>
                            <asp:TextBox runat="server" ID="txtSendEmailCC" TextMode="MultiLine" CssClass="form-control"  Height="135px" placeholder="CC ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                            <p><small class="text-muted">* Please enter the required email address in a new line.</small></p>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorSendEmail">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorSendEmail"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSendEmail" CssClass="btn btn-primary" Text="Submit" OnClick="btnSendEmail_Click" OnClientClick="return showWaiting();" />
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

        function showSendEmail() {
            $("#modalSendEmail").modal("show");
        }

        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 1000);

            return true;
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

        ["modalLog", "modalSendEmail", "modalWaiting"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
