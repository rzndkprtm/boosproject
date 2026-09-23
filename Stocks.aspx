<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Stocks.aspx.vb" Inherits="Stocks" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Stocks" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .grid-container { width: 100%; height: calc(100vh - 150px); overflow: auto; border: 1px solid #ddd; }
        .grid-container table { width: 100%; border-collapse: collapse; table-layout: auto; min-height: 180%; }
        .grid-container td, .grid-container th { white-space: nowrap; padding: 6px 10px; }
        .grid-container th { position: sticky; top: 0; background: #f5f5f5; z-index: 3; }
        .grid-container td:first-child, .grid-container th:first-child { position: sticky; left: 0; background: #fff; z-index: 2; white-space: normal; word-break: break-word; min-width: 220px; max-width: 320px; }
        .grid-container th:first-child { z-index: 5; background: #f5f5f5; }
        .grid-container tr:last-child td { position: sticky; bottom: 0; background: #f5f5f5; z-index: 3; }
        .grid-container tr:last-child td:first-child { left: 0; z-index: 6; background: #f5f5f5; }
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
                <div class="card">
                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-body">
                                <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                    <a class="list-group-item list-group-item-action active" id="listRoller" data-bs-toggle="list" href="#list-roller" role="tab">Roller</a>
                                    <a class="list-group-item list-group-item-action" id="listVertical" data-bs-toggle="list" href="#list-vertical" role="tab">Vertical</a>
                                    <a class="list-group-item list-group-item-action" id="listCellular" data-bs-toggle="list" href="#list-cellular" role="tab">Cellular Shades</a>
                                    <a class="list-group-item list-group-item-action" id="listDesignShades" data-bs-toggle="list" href="#list-designshades" role="tab">Design Shades</a>
                                    <a class="list-group-item list-group-item-action" id="listCurtain" data-bs-toggle="list" href="#list-curtain" role="tab">Curtain</a>
                                    <a class="list-group-item list-group-item-action" id="listVenetian" data-bs-toggle="list" href="#list-venetian" role="tab">Venetian Blind</a>
                                    <a class="list-group-item list-group-item-action" id="listAluminium" data-bs-toggle="list" href="#list-aluminium" role="tab">Aluminium Blind</a>
                                    <a class="list-group-item list-group-item-action" id="listFabricChart" data-bs-toggle="list" href="#list-fabricchart" role="tab">Fabric Chart</a>
                                </div>
                                <div class="tab-content text-justify">
                                    <div class="tab-pane fade show active" id="list-roller" role="tabpanel" aria-labelledby="listRoller">
                                        <div class="row mt-5" runat="server" id="divErrorRoller">
                                            <div class="col-12">
                                                <div class="alert alert-danger"><span runat="server" id="msgErrorRoller"></span></div>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12 col-sm-12 col-lg-7">
                                                <asp:Panel runat="server" DefaultButton="btnSearchRoller" Width="100%">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Fabric Type : </span>
                                                        <asp:TextBox runat="server" ID="txtSearchRoller" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                        <asp:Button runat="server" ID="btnSearchRoller" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchRoller_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="accordion" id="accordionRoller">
                                                    <asp:Repeater runat="server" ID="rptRoller" OnItemDataBound="rptRoller_ItemDataBound">
                                                        <ItemTemplate>
                                                            <div class="accordion-item">
                                                                <h2 class="accordion-header" id="headingRoller_<%# Container.ItemIndex %>">
                                                                    <button type="button" class="accordion-button <%# If(Container.ItemIndex = 0, "", "collapsed") %>" data-bs-toggle="collapse" data-bs-target="#collapseRoller_<%# Container.ItemIndex %>" aria-expanded="<%# If(Container.ItemIndex = 0, "true", "false") %>" aria-controls="collapseRoller_<%# Container.ItemIndex %>"><strong><%# Eval("Name") %></strong></button>
                                                                </h2>
                                                                <div id="collapseRoller_<%# Container.ItemIndex %>" class="accordion-collapse collapse <%# If(Container.ItemIndex = 0, "show", "") %>" aria-labelledby="headingRoller_<%# Container.ItemIndex %>" data-bs-parent="#accordionRoller">
                                                                    <div class="accordion-body">
                                                                        <asp:Repeater ID="rptRollerColour" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="border rounded p-3 mb-2">
                                                                                    <div class="row">
                                                                                        <div class="col-md-3"><div class="text-muted small">Colour</div><strong><%# Eval("Colour").ToString.ToUpper() %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Status</div><span class="<%# GetStatusClass(Eval("Status")) %>"><%# Eval("Status") %></span></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Maximum Width</div><strong><%# Eval("Width") %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Roll QTY</div><strong><%# Eval("RollQty") %></strong></div>
                                                                                        <div class="col-md-3"><div class="text-muted small">ETA Factory</div><strong><%# Eval("ETAFactory", "{0:dd MMM yyyy}") %></strong></div>
                                                                                    </div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                                <asp:Panel ID="pnlNoDataRoller" runat="server" Visible="false" CssClass="text-center text-muted py-5">No fabric stock found.</asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-vertical" role="tabpanel" aria-labelledby="listVertical">
                                        <div class="row mt-5" runat="server" id="divErrorVertical">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorVertical"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12 col-sm-12 col-lg-7">
                                                <asp:Panel runat="server" DefaultButton="btnSearchVertical" Width="100%">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Fabric Type : </span>
                                                        <asp:TextBox runat="server" ID="txtSearchVertical" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                        <asp:Button runat="server" ID="btnSearchVertical" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchVertical_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="accordion" id="accordionVertical">
                                                    <asp:Repeater runat="server" ID="rptVertical" OnItemDataBound="rptVertical_ItemDataBound">
                                                        <ItemTemplate>
                                                            <div class="accordion-item">
                                                                <h2 class="accordion-header" id="headingVertical_<%# Container.ItemIndex %>">
                                                                    <button type="button" class="accordion-button <%# If(Container.ItemIndex = 0, "", "collapsed") %>" data-bs-toggle="collapse" data-bs-target="#collapseVertical_<%# Container.ItemIndex %>" aria-expanded="<%# If(Container.ItemIndex = 0, "true", "false") %>" aria-controls="collapseVertical_<%# Container.ItemIndex %>"><strong><%# Eval("Name") %></strong></button>
                                                                </h2>
                                                                <div id="collapseVertical_<%# Container.ItemIndex %>" class="accordion-collapse collapse <%# If(Container.ItemIndex = 0, "show", "") %>" aria-labelledby="headingVertical_<%# Container.ItemIndex %>" data-bs-parent="#accordionVertical">
                                                                    <div class="accordion-body">
                                                                        <asp:Repeater ID="rptVerticalColour" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="border rounded p-3 mb-2">
                                                                                    <div class="row">
                                                                                        <div class="col-md-3"><div class="text-muted small">Colour</div><strong><%# Eval("Colour").ToString.ToUpper() %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Status</div><span class="<%# GetStatusClass(Eval("Status")) %>"><%# Eval("Status") %></span></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Roll QTY</div><strong><%# Eval("RollQty") %></strong></div>
                                                                                        <div class="col-md-3"><div class="text-muted small">ETA Factory</div><strong><%# Eval("ETAFactory", "{0:dd MMM yyyy}") %></strong></div>
                                                                                    </div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                                <asp:Panel ID="pnlNoDataVertical" runat="server" Visible="false" CssClass="text-center text-muted py-5">No fabric stock found.</asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-cellular" role="tabpanel" aria-labelledby="listCellular">
                                        <div class="row mt-5" runat="server" id="divErrorCellular">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorCellular"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="accordion" id="accordionCellular">
                                                    <asp:Repeater runat="server" ID="rptCellular" OnItemDataBound="rptCellular_ItemDataBound">
                                                        <ItemTemplate>
                                                            <div class="accordion-item">
                                                                <h2 class="accordion-header" id="headingCellular_<%# Container.ItemIndex %>">
                                                                    <button type="button" class="accordion-button <%# If(Container.ItemIndex = 0, "", "collapsed") %>" data-bs-toggle="collapse" data-bs-target="#collapseCellular_<%# Container.ItemIndex %>" aria-expanded="<%# If(Container.ItemIndex = 0, "true", "false") %>" aria-controls="collapseCellular_<%# Container.ItemIndex %>"><strong><%# Eval("Name") %></strong></button>
                                                                </h2>
                                                                <div id="collapseCellular_<%# Container.ItemIndex %>" class="accordion-collapse collapse <%# If(Container.ItemIndex = 0, "show", "") %>" aria-labelledby="headingCellular_<%# Container.ItemIndex %>" data-bs-parent="#accordionCellular">
                                                                    <div class="accordion-body">
                                                                        <asp:Repeater ID="rptCellularColour" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="border rounded p-3 mb-2">
                                                                                    <div class="row">
                                                                                        <div class="col-md-3"><div class="text-muted small">Colour</div><strong><%# Eval("Colour").ToString.ToUpper() %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Status</div><span class="<%# GetStatusClass(Eval("Status")) %>"><%# Eval("Status") %></span></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Maximum Width</div><strong><%# Eval("Width") %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Roll QTY</div><strong><%# Eval("RollQty") %></strong></div>
                                                                                        <div class="col-md-3"><div class="text-muted small">ETA Factory</div><strong><%# Eval("ETAFactory", "{0:dd MMM yyyy}") %></strong></div>
                                                                                    </div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                                <asp:Panel ID="pnlNoDataCellular" runat="server" Visible="false" CssClass="text-center text-muted py-5">No fabric stock found.</asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-designshades" role="tabpanel" aria-labelledby="listDesignShades">
                                        <div class="row mt-5" runat="server" id="divErrorDesignShades">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorDesignShades"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="accordion" id="accordionDesignShades">
                                                    <asp:Repeater runat="server" ID="rptDesignShades" OnItemDataBound="rptDesignShades_ItemDataBound">
                                                        <ItemTemplate>
                                                            <div class="accordion-item">
                                                                <h2 class="accordion-header" id="headingDesignShades_<%# Container.ItemIndex %>">
                                                                    <button type="button" class="accordion-button <%# If(Container.ItemIndex = 0, "", "collapsed") %>" data-bs-toggle="collapse" data-bs-target="#collapseDesignShades_<%# Container.ItemIndex %>" aria-expanded="<%# If(Container.ItemIndex = 0, "true", "false") %>" aria-controls="collapseDesignShades_<%# Container.ItemIndex %>"><strong><%# Eval("Name") %></strong></button>
                                                                </h2>
                                                                <div id="collapseDesignShades_<%# Container.ItemIndex %>" class="accordion-collapse collapse <%# If(Container.ItemIndex = 0, "show", "") %>" aria-labelledby="headingDesignShades_<%# Container.ItemIndex %>" data-bs-parent="#accordionDesignShades">
                                                                    <div class="accordion-body">
                                                                        <asp:Repeater ID="rptDesignShadesColour" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="border rounded p-3 mb-2">
                                                                                    <div class="row">
                                                                                        <div class="col-md-3"><div class="text-muted small">Colour</div><strong><%# Eval("Colour").ToString.ToUpper() %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Status</div><span class="<%# GetStatusClass(Eval("Status")) %>"><%# Eval("Status") %></span></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Maximum Width</div><strong><%# Eval("Width") %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Roll QTY</div><strong><%# Eval("RollQty") %></strong></div>
                                                                                        <div class="col-md-3"><div class="text-muted small">ETA Factory</div><strong><%# Eval("ETAFactory", "{0:dd MMM yyyy}") %></strong></div>
                                                                                    </div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                                <asp:Panel ID="pnlNoDataDesignShades" runat="server" Visible="false" CssClass="text-center text-muted py-5">No fabric stock found.</asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-curtain" role="tabpanel" aria-labelledby="listCurtain">
                                        <div class="row mt-5" runat="server" id="divErrorCurtain">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorCurtain"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12 col-sm-12 col-lg-7">
                                                <asp:Panel runat="server" DefaultButton="btnSearchCurtain" Width="100%">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Fabric Type : </span>
                                                        <asp:TextBox runat="server" ID="txtSearchCurtain" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                        <asp:Button runat="server" ID="btnSearchCurtain" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchCurtain_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <div class="accordion" id="accordionCurtain">
                                                    <asp:Repeater runat="server" ID="rptCurtain" OnItemDataBound="rptCurtain_ItemDataBound">
                                                        <ItemTemplate>
                                                            <div class="accordion-item">
                                                                <h2 class="accordion-header" id="headingCurtain_<%# Container.ItemIndex %>">
                                                                    <button type="button" class="accordion-button <%# If(Container.ItemIndex = 0, "", "collapsed") %>" data-bs-toggle="collapse" data-bs-target="#collapseCurtain_<%# Container.ItemIndex %>" aria-expanded="<%# If(Container.ItemIndex = 0, "true", "false") %>" aria-controls="collapseCurtain_<%# Container.ItemIndex %>"><strong><%# Eval("Name") %></strong></button>
                                                                </h2>
                                                                <div id="collapseCurtain_<%# Container.ItemIndex %>" class="accordion-collapse collapse <%# If(Container.ItemIndex = 0, "show", "") %>" aria-labelledby="headingCurtain_<%# Container.ItemIndex %>" data-bs-parent="#accordionCurtain">
                                                                    <div class="accordion-body">
                                                                        <asp:Repeater ID="rptCurtainColour" runat="server">
                                                                            <ItemTemplate>
                                                                                <div class="border rounded p-3 mb-2">
                                                                                    <div class="row">
                                                                                        <div class="col-md-3"><div class="text-muted small">Colour</div><strong><%# Eval("Colour").ToString.ToUpper() %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Status</div><span class="<%# GetStatusClass(Eval("Status")) %>"><%# Eval("Status") %></span></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Maximum Width</div><strong><%# Eval("Width") %></strong></div>
                                                                                        <div class="col-md-2"><div class="text-muted small">Roll QTY</div><strong><%# Eval("RollQty") %></strong></div>
                                                                                        <div class="col-md-3"><div class="text-muted small">ETA Factory</div><strong><%# Eval("ETAFactory", "{0:dd MMM yyyy}") %></strong></div>
                                                                                    </div>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                        </asp:Repeater>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                                <asp:Panel ID="pnlNoDataCurtain" runat="server" Visible="false" CssClass="text-center text-muted py-5">No fabric stock found.</asp:Panel>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-venetian" role="tabpanel" aria-labelledby="listVenetian">
                                        <div class="row mt-5" runat="server" id="divErrorVenetian">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorVenetian"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <table class="table mb-0">
                                                        <tbody>
                                                            <tr>
                                                                <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <div class="table-responsive grid-container">
                                                    <asp:GridView runat="server" ID="gvListVenetian" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListVenetian_RowDataBound">
                                                        <RowStyle />
                                                        <Columns>
                                                            <asp:BoundField DataField="BlindName" HeaderText="" />
                                                            <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-aluminium" role="tabpanel" aria-labelledby="listAluminium">
                                        <div class="row mt-5" runat="server" id="divErrorAluminium">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorAluminium"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <table class="table mb-0">
                                                        <tbody>
                                                            <tr>
                                                                <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                            </tr>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <div class="table-responsive grid-container">
                                                    <asp:GridView runat="server" ID="gvListAluminium" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListAluminium_RowDataBound">
                                                        <RowStyle />
                                                        <Columns>
                                                            <asp:BoundField DataField="BlindName" HeaderText="" />
                                                            <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="tab-pane fade" id="list-fabricchart" role="tabpanel" aria-labelledby="listFabricChart">
                                        <div class="row mt-5" runat="server" id="divErrorFabricChart">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorFabricChart"></span>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mt-5" runat="server">
                                            <div class="col-12 col-sm-12 col-lg-7">
                                                <asp:Panel runat="server" DefaultButton="btnFabricChart" Width="100%">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Fabric Type : </span>
                                                        <asp:TextBox runat="server" ID="txtSearchFabricChart" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                        <asp:Button runat="server" ID="btnFabricChart" CssClass="btn btn-primary" Text="Search" OnClick="btnFabricChart_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <div class="table-responsive grid-container">
                                                    <asp:GridView runat="server" ID="gvListFabricChart" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListFabricChart_RowDataBound">
                                                        <RowStyle />
                                                        <Columns>
                                                            <asp:BoundField DataField="FabricName" HeaderText="Fabric Type" />
                                                            <asp:BoundField DataField="Roller" HeaderText="Roller" />
                                                            <asp:BoundField DataField="RomanClassic" HeaderText="Roman (Classic)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            <asp:BoundField DataField="RomanPlantation" HeaderText="Roman (Plantation)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            <asp:BoundField DataField="RomanSewless" HeaderText="Roman (Sewless)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            <asp:BoundField DataField="PGPlain" HeaderText="PG (Plain)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            <asp:BoundField DataField="PGPlantation" HeaderText="PG (Plantation)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            <asp:BoundField DataField="PGSewless" HeaderText="PG (Sewless)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            <asp:BoundField DataField="Vertical89mm" HeaderText="Vertical (89mm)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                            <asp:BoundField DataField="Vertical127mm" HeaderText="Vertical (127mm)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer" runat="server" id="divCompanyDetail">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-4 d-flex justify-content-end">
                                        <div class="input-group">
                                            <span class="input-group-text">Company Detail</span>
                                            <asp:DropDownList runat="server" ID="ddlCompanyDetail" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompanyDetail_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>                    
                </div>
            </div>
        </section>
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

    <asp:HiddenField ID="selected_tab" runat="server" />

    <script type="text/javascript">
        function showLoading() {
            $("#loadingOverlay").show();
        }
        function hideLoading() {
            $("#loadingOverlay").hide();
        }
        function activateCurrentTab() {
            var tabId = $("#<%= selected_tab.ClientID %>").val();
            if (!tabId) tabId = "list-roller";
            $('#dvTab a[href="#' + tabId + '"]').tab('show');
        }
        function pageInit() {
            $(document).off("click.stocktab").on("click.stocktab", "#dvTab a", function () {
                var tabId = $(this).attr("href").replace("#", "");
                $("#<%= selected_tab.ClientID %>").val(tabId);
            });
            activateCurrentTab();
        }
        $(document).ready(function () {
            pageInit();
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                showLoading();
            });
            prm.add_endRequest(function () {
                hideLoading();
                pageInit();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
