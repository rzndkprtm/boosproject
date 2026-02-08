Imports System.Data
Imports System.Data.SqlClient

Partial Class Sales_Product
    Inherits Page
    
    Dim salesClass As New SalesClass
    Dim mailingClass As New MailingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataMailing As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            MessageError_Custom(False, String.Empty)

            BindData()
            BindDesign()
        End If
    End Sub

    Protected Sub btnCustom_Click(sender As Object, e As EventArgs)
        MessageError_Custom(False, String.Empty)
        Dim thisScript As String = "window.onload = function() { showCustom(); };"
        Try
            BindData()
        Catch ex As Exception
            ClientScript.RegisterStartupScript(Me.GetType(), "showCustom", thisScript, True)
        End Try
    End Sub

    Protected Sub gvList_RowCreated(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow OrElse e.Row.RowType = DataControlRowType.Header Then
            e.Row.Cells(0).Visible = False

            For i As Integer = 0 To e.Row.Cells.Count - 1
                If e.Row.Cells(i).Text = "CustomerName" Then
                    e.Row.Cells(i).Text = "Customer Name"
                End If
            Next
        End If
    End Sub

    Protected Sub tmrTicket_Tick(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindData()
    End Sub

    Protected Sub BindData()
        Try
            Dim myCmd As New SqlCommand("sp_ProductionSummaryBlindsPivot_ByArea")
            myCmd.CommandType = CommandType.StoredProcedure

            Dim area As String = String.Empty
            For Each item As ListItem In lbArea.Items
                If item.Selected Then
                    area += item.Value & ","
                End If
            Next
            Dim areaList As String = String.Empty
            If Not String.IsNullOrEmpty(area) Then
                areaList = area.Remove(area.Length - 1).ToString()
            End If

            Dim design As String = String.Empty
            For Each item As ListItem In lbDesign.Items
                If item.Selected Then
                    design += item.Value & ","
                End If
            Next

            Dim designId As String = String.Empty
            If Not String.IsNullOrEmpty(design) Then
                designId = design.Remove(design.Length - 1).ToString()
            End If

            myCmd.Parameters.AddWithValue("@StartDate", DateTime.Now)
            myCmd.Parameters.AddWithValue("@EndDate", DateTime.Now)
            myCmd.Parameters.AddWithValue("@CompanyId", "2")
            myCmd.Parameters.AddWithValue("@DesignIds", If(String.IsNullOrEmpty(designId), CType(DBNull.Value, Object), designId))
            myCmd.Parameters.AddWithValue("@AreaList", If(String.IsNullOrEmpty(areaList), CType(DBNull.Value, Object), areaList))

            Dim ds As DataSet = salesClass.GetProductSales(myCmd)
            gvList.DataSource = ds
            gvList.DataBind()
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindDesign()
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = salesClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 0 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            lbDesign.Items.Clear()
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Sub MessageError_Custom(visible As Boolean, message As String)
        divErrorCustom.Visible = visible : msgErrorCustom.InnerText = message
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
