<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Specification_Bottom_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Bottom Detail" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification">Specification</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/bottom">Bottom</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row">
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
            <div class="col-12 col-sm-12 col-lg-4">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="form form-vertical">
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>ID</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblId" CssClass="form-label font-bold"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Bottom Name</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblName" CssClass="form-label font-bold"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Design Type</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblDesignType" CssClass="form-label font-bold"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Company Detail</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblCompanyDetail" CssClass="form-label font-bold"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Description</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblDescription" CssClass="form-label font-bold"></asp:Label>
                                    </div>
                                </div>
                                <div class="row mb-2">
                                    <div class="col-12">
                                        <label>Active</label>
                                        <br />
                                        <asp:Label runat="server" ID="lblActive" CssClass="form-label font-bold"></asp:Label>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="card-footer text-center">
                        <asp:Button runat="server" ID="btnEdit" CssClass="btn btn-info" Text="Edit" OnClick="btnEdit_Click" />
                        <a href="#" runat="server" id="aActive" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalActive" onclick="return showActive()"><%= TextActive(lblActive.Text) %></a>
                        <a href="javascript:void(0);" class="btn btn-secondary" onclick="showLog('Bottoms', '<%= lblId.Text %>')">Log</a>
                    </div>
                </div>
            </div>
            <div class="col-12 col-sm-12 col-lg-8">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-6">
                                    <h3 class="card-title">List Colour</h3>
                                </div>
                                <div class="col-6 d-flex justify-content-end">
                                    <asp:Button runat="server" ID="btnAddColour" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddColour_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="BoeId" HeaderText="BOE ID" />
                                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                                <asp:BoundField DataField="Colour" HeaderText="Colour" />
                                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                                <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li runat="server" visible='<%# PageAction("Detail Colour") %>'>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('BottomColours', '<%# Eval("Id") %>')">Log</a>
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
                </div>
            </div>
        </section>
    </div>

    <div class="modal modal-blur fade" id="modalActive" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white" id="titleActive"></h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnActive" CssClass="btn btn-warning" Text="Confirm" OnClick="btnActive_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalProcessColour" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 runat="server" class="modal-title" id="titleProcessColour"></h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                            <label class="form-label">BOE ID</label>
                            <asp:TextBox runat="server" ID="txtBoeId" CssClass="form-control" placeholder="BOE ID ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-8 form-group">
                            <label class="form-label">Colour</label>
                            <asp:TextBox runat="server" ID="txtColour" CssClass="form-control" placeholder="Colour ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                
                    <div class="row mb-2" runat="server" id="divActive">
                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                            <label class="form-label">Active</label>
                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorProcessColour">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessColour"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessColour" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcessColour_Click" />
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
        <asp:Label runat="server" ID="lblIdColour"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>

    <script type="text/javascript">
        function showActive(id) {
            var active = document.getElementById("<%=lblActive.ClientID %>").innerText;
            let title = "";
            if (active === "Yes") {
                title = "Deactivate Bottom Type";
            } else {
                title = "Activate Bottom Type";
            }
            document.getElementById("titleActive").innerHTML = title;
        }

        function showProcessColour() {
            $("#modalProcessColour").modal("show");
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

        ["modalActive", "modalProcessColour", "modalLog"].forEach(function (id) {
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

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>