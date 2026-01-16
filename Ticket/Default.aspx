<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Ticket_Default" MasterPageFile="~/Site.master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Ticket" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <asp:Timer runat="server" ID="tmrTicket" Interval="10000" OnTick="tmrTicket_Tick" />
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p runat="server" id="pPageInfo" class="text-subtitle text-muted">
                        This page allows you to create and track tickets for issues, questions, or support requests related to the system.<br />
                        Our team will review and follow up on each ticket accordingly.
                    </p>
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
            <div class="col-12 d-flex justify-content-end flex-wrap gap-2">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Open Ticket" OnClick="btnAdd_Click" />
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
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-7 mb-2"></div>
                                <div class="col-12 col-sm-12 col-lg-5 d-flex justify-content-end mb-2">
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
                        
                        <asp:UpdatePanel runat="server" ID="upDetail" UpdateMode="Conditional">
                            <ContentTemplate>
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
                                                        <asp:BoundField DataField="TicketCode" HeaderText="Ticket Code" />
                                                        <asp:BoundField DataField="Issue" HeaderText="Issue" />
                                                        <asp:BoundField DataField="Subject" HeaderText="Subject" />
                                                        <asp:BoundField DataField="FullName" HeaderText="Created By" />
                                                        <asp:BoundField DataField="CreatedDate" HeaderText="Created" DataFormatString="{0:dd MMM yyyy HH:mm}" />
                                                        <asp:BoundField DataField="TicketStatus" HeaderText="Status" />
                                                        <asp:TemplateField ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
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
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="tmrTicket" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    
                    <div class="card-footer"></div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Open Ticket</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2" runat="server" visible="false">
                        <div class="col-12 form-group">
                            <label class="form-label">Your Email</label>
                            <asp:TextBox runat="server" ID="txtCreatedEmail" CssClass="form-control" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-4 form-group">
                            <label class="form-label">Issue</label>
                            <asp:DropDownList runat="server" ID="ddlIssue" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Web Issue" Text="Web Issue"></asp:ListItem>
                                <asp:ListItem Value="Product Issue" Text="Product Issue"></asp:ListItem>
                                <asp:ListItem Value="Pricing Issue" Text="Pricing Issue"></asp:ListItem>
                                <asp:ListItem Value="Other Issue" Text="Other Issue"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-8 form-group">
                            <label class="form-label">Subject</label>
                            <asp:TextBox runat="server" ID="txtSubject" CssClass="form-control" placeholder="Subject ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Message</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtMessage" Height="220px" CssClass="form-control" placeholder="Your Message ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Attachment</label>
                            <asp:FileUpload runat="server" ID="fuAttachment" CssClass="form-control" AllowMultiple="true" onchange="showSelectedFiles(this)" />

                            <div id="fileList" style="margin-top:10px; font-family:Cambria; font-size:14px; color:#333;"></div>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcess"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcess" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcess_Click" OnClientClick="return showWaiting($(this).closest('.modal').attr('id'));" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalEmail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Personal Email</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />A personal email address is required.<br />Please update and complete the information first.
                </div>
                <div class="modal-footer">
                    <a runat="server" href="~/order" class="btn btn-light-secondary">Cancel</a>
                    <a runat="server" href="~/account" class="btn btn-danger">Setting</a>
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
        function showSelectedFiles(input) {
            const list = document.getElementById("fileList");
            list.innerHTML = "";

            if (input.files.length > 0) {
                const ul = document.createElement("ul");
                ul.style.listStyleType = "disc";
                ul.style.paddingLeft = "20px";

                for (let i = 0; i < input.files.length; i++) {
                    const li = document.createElement("li");
                    li.textContent = input.files[i].name;
                    ul.appendChild(li);
                }

                list.innerHTML = "<b>📎 Files selected:</b>";
                list.appendChild(ul);
            } else {
                list.innerHTML = "<i>No file selected.</i>";
            }
        }

        Sys.Application.add_load(function () {
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
                    ) return;

                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                });
            }
        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            document.title = '<%= Page.Title %> - <%= Session("CompanyName") %>';
        });

        function showProcess() {
            $("#modalProcess").modal("show");
        }

        function showEmail() {
            $("#modalEmail").modal("show");
        }

        ["modalProcess", "modalWaiting"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

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
