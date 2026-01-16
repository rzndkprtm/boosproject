Imports System.Globalization

Partial Class Sales_Costing
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing

    Dim salesClass As New SalesClass
    Dim mailingClass As New MailingClass

    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(txtSearch.Text)
        End If
    End Sub

    Protected Sub tmrTicket_Tick(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub btnSearch_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData(txtSearch.Text)
    End Sub

    Protected Sub gvList_PageIndexChanging(sender As Object, e As GridViewPageEventArgs)
        MessageError(False, String.Empty)
        Try
            gvList.PageIndex = e.NewPageIndex
            BindData(txtSearch.Text)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub gvList_RowCommand(sender As Object, e As GridViewCommandEventArgs)
        If Not String.IsNullOrEmpty(e.CommandArgument) Then
            Dim dataId As String = e.CommandArgument.ToString()
            If e.CommandName = "Refresh" Then
                MessageError(False, String.Empty)
                Try
                    salesClass.RefreshData(dataId)

                    Response.Redirect("~/sales/costing", False)
                Catch ex As Exception
                    MessageError(True, ex.ToString())
                End Try
            End If
        End If
    End Sub

    Protected Sub BindData(searchDate As String)
        Try
            Dim dateValue As Date

            Dim search As String = String.Empty

            If Not String.IsNullOrWhiteSpace(searchDate) AndAlso Date.TryParse(searchDate, dateValue) Then
                search = " WHERE SummaryDate = '" & dateValue.ToString("yyyy-MM-dd") & "'"
            End If

            Dim thisQuery As String = String.Format("SELECT * FROM Sales {0} ORDER BY SummaryDate DESC", search)

            gvList.DataSource = salesClass.GetListData(thisQuery)
            gvList.DataBind()

            gvList.Columns(1).Visible = PageAction("Visible ID")
            gvList.Columns(8).Visible = PageAction("Visible Action")
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Function BindPrice(price As Decimal) As String
        If price > 0 Then
            Return String.Format("$ {0}", price.ToString("N2", enUS))
        End If
        Return "$ 0.00"
    End Function

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
