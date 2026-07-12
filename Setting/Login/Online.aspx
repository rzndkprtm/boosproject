<%@ Page Language="VB" AutoEventWireup="false" ValidateRequest="false" CodeFile="Online.aspx.vb" Inherits="Setting_Login_Online" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Login Online" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/login">Login</a></li>
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
                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                        <h4 class="card-title">List User</h4>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                        <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                            <div class="input-group">
                                                <span class="input-group-text">Search : </span>
                                                <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                            </div>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="100" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Visible="false" OnPageIndexChanging="gvList_PageIndexChanging" OnDataBound="gvList_DataBound">
                                        <Columns>
                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                            <asp:BoundField DataField="UserName" HeaderText="UserName" />
                                            <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                                            <asp:BoundField DataField="RoleName" HeaderText="Role Access" />
                                            <asp:BoundField DataField="LastLogin" HeaderText="Last Login" DataFormatString="{0:dd MMM yyyy HH:mm:ss}" />
                                            <asp:BoundField DataField="LastActiveMinute" HeaderText="Active (Minute Ago)" />
                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                <ItemTemplate>
                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                    <ul class="dropdown-menu">
                                                        <li>
                                                            <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalSendNotif" onclick='<%# String.Format("return idSendNotif(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("RoleId").ToString()) %>'>Send Notification</a>
                                                        </li>
                                                        <li runat="server" visible='<%# LoginAccess("Delete Session") %>'>
                                                            <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteSession" onclick='<%# String.Format("return dataDeleteSession(`{0}`);", Eval("Id").ToString()) %>'>Delete Session</a>
                                                        </li>
                                                    </ul>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                                <div class="d-flex justify-content-end mt-3">
                                    <nav id="navPager" runat="server" visible="false">
                                        <ul class="pagination pagination mb-0">
                                            <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
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
                            <div class="card-footer">
                                <div class="row" runat="server" id="divMinute">
                                    <div class="col-12 col-sm-12 col-lg-2">
                                        <div class="input-group">
                                            <span class="input-group-text">Minute : </span>
                                            <asp:DropDownList runat="server" ID="ddlMinute" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlMinute_SelectedIndexChanged">
                                                <asp:ListItem Value="5" Text="5"></asp:ListItem>
                                                <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                                <asp:ListItem Value="15" Text="15"></asp:ListItem>
                                                <asp:ListItem Value="20" Text="20"></asp:ListItem>
                                                <asp:ListItem Value="25" Text="25"></asp:ListItem>
                                                <asp:ListItem Value="30" Text="30"></asp:ListItem>
                                                <asp:ListItem Value="35" Text="35"></asp:ListItem>
                                                <asp:ListItem Value="40" Text="40"></asp:ListItem>
                                                <asp:ListItem Value="45" Text="45"></asp:ListItem>
                                                <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                                <asp:ListItem Value="55" Text="55"></asp:ListItem>
                                                <asp:ListItem Value="60" Text="60"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalSendNotif" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Send Notification</h4>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtLoginId" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtRoleId" style="display:none;"></asp:TextBox>

                    <div class="row mb-2" runat="server" id="divErrorSendNotif">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorSendNotif"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Title</label>
                            <asp:TextBox runat="server" ID="txtTitle" CssClass="form-control" placeholder="Title ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Message</label>
                            <div id="summernote"></div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSendNotif" CssClass="btn btn-info" Text="Submit" OnClick="btnSendNotif_Click" OnClientClick="return setSummernoteContent();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalDeleteSession" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Session</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtDeleteId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteSession" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteSession_Click" />
                </div>
            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="fieldMessage" />
    <asp:HiddenField runat="server" ID="hfMessage" />

    <script type="text/javascript">
        $(document).ready(function () {
            $('#summernote').summernote({
                tabsize: 2, height: 350,
                callbacks: {
                    onChange: function (contents) {
                        $('#<%= hfMessage.ClientID %>').val(contents);
                    }
                }
            });

            $('#summernote').summernote('code', $('#<%= hfMessage.ClientID %>').val());
        });
        function setSummernoteContent() {
            var content = $('#summernote').summernote('code');
            $('#<%= fieldMessage.ClientID %>').val(content);
            return true;
        }
        function showSendNotif() {
            $("#modalSendNotif").modal("show");
        }
        function idSendNotif(loginId, roleId) {
            document.getElementById("<%=txtLoginId.ClientID %>").value = loginId;
            document.getElementById("<%=txtRoleId.ClientID %>").value = roleId;
        }
        function dataDeleteSession(id) {
            document.getElementById("<%=txtDeleteId.ClientID %>").value = id;
        }
        ["modalSendNotif", "modalDeleteSession"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>