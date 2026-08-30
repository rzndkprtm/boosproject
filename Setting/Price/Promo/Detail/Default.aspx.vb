Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization

Partial Class Setting_Price_Promo_Detail_Default
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty
    Dim dataLog As Object()
    Dim enUS As CultureInfo = New CultureInfo("en-US")

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/promo", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("promoid")) Then
            Response.Redirect("~/setting/price/promo", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("promoid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnEdit_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/price/promo/edit?promoid={0}&returnpage=detail", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnDelete_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM Promos WHERE Id=@Id; DELETE FROM Logs WHERE Type='Promos' AND DataId=@Id;", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim customerPromos As DataTable = settingClass.GetDataTable("SELECT Id FROM CustomerPromos WHERE PromoId='" & lblId.Text & "'")
            If customerPromos.Rows.Count > 0 Then
                For i As Integer = 0 To customerPromos.Rows.Count - 1
                    Dim thisId As String = customerPromos.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM CustomerPromos WHERE Id=@Id; DELETE FROM Logs WHERE Type='CustomerPromos' AND DataId=@Id;", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                Next
            End If

            Dim promoDetail As DataTable = settingClass.GetDataTable("SELECT Id FROM PromoDetails WHERE PromoId='" & lblId.Text & "'")
            If promoDetail.Rows.Count > 0 Then
                For i As Integer = 0 To promoDetail.Rows.Count - 1
                    Dim thisId As String = promoDetail.Rows(i)("Id").ToString()

                    Using thisConn As New SqlConnection(myConn)
                        Using thisCmd As SqlCommand = New SqlCommand("DELETE FROM PromoDetails WHERE Id=@Id; DELETE FROM Logs WHERE Type='PromoDetails' AND DataId=@Id;", thisConn)
                            thisCmd.Parameters.AddWithValue("@Id", thisId)
                            thisConn.Open()
                            thisCmd.ExecuteNonQuery()
                        End Using
                    End Using
                Next
            End If

            Response.Redirect("~/setting/price/promo", False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnAddDetail_Click(sender As Object, e As EventArgs)
        Dim url As String = String.Format("~/setting/price/promo/detail/add?promoid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub btnChangeDetail_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataId As String = txtChangeValueId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE PromoDetails SET Discount=@Discount WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", dataId)
                    thisCmd.Parameters.AddWithValue("@Discount", txtDiscount.Text)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"PromoDetails", dataId, Session("LoginId").ToString(), "Price Promo Detail Change Value"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/price/promo/detail?promoid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnStatusDetail_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim thisId As String = txtStatusDetailId.Text
            Dim thisStatus As String = txtStatusDetailText.Text

            Dim newStatus As String = "Inactive"
            If thisStatus = "Inactive" Then : newStatus = "Active" : End If

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE PromoDetails SET Status=@Status WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", thisId)
                    thisCmd.Parameters.AddWithValue("@Status", newStatus)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            Dim statusDesc As String = "Promo Has Been Activated"
            If newStatus = "Inactive" Then statusDesc = "Promo Has Been Deactivated"

            dataLog = {"PromoDetails", thisId, Session("LoginId").ToString(), statusDesc}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/price/promo/detail?promoid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnDeleteDetail_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            Dim dataId As String = txtDeleteDetailId.Text

            Using thisConn As New SqlConnection(myConn)
                Using thisCmd As SqlCommand = New SqlCommand("UPDATE PromoDetails SET Status='Deleted' WHERE Id=@Id", thisConn)
                    thisCmd.Parameters.AddWithValue("@Id", dataId)
                    thisConn.Open()
                    thisCmd.ExecuteNonQuery()
                End Using
            End Using

            dataLog = {"PromoDetails", dataId, Session("LoginId").ToString(), "Price Promo Detail Deleted"}
            settingClass.Logs(dataLog)

            url = String.Format("~/setting/price/promo/detail?promoid={0}", lblId.Text)
            Response.Redirect(url, False)
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindData(promoId As String)
        Try
            Dim params As New List(Of SqlParameter) From {
                New SqlParameter("@PromoId", promoId),
                New SqlParameter("@RoleName", Session("RoleName")),
                New SqlParameter("@LevelName", Session("LevelName")),
                New SqlParameter("@CompanyId", If(String.IsNullOrWhiteSpace(Session("CompanyId")), CType(DBNull.Value, Object), Session("CompanyId").ToString()))
            }

            Dim thisData As DataRow = settingClass.GetDataRowSP("sp_Promos_GetById", params)
            If thisData Is Nothing Then
                Response.Redirect("~/setting/price/promo", False)
                Exit Sub
            End If

            lblCompanyName.Text = thisData("CompanyName").ToString()
            lblCompanyId.Text = thisData("CompanyId").ToString()
            lblName.Text = thisData("Name").ToString()
            lblStartDate.Text = Convert.ToDateTime(thisData("StartDate")).ToString("dd MMM yyyy")
            lblEndDate.Text = Convert.ToDateTime(thisData("EndDate")).ToString("dd MMM yyyy")
            lblType.Text = thisData("Type").ToString()
            lblDescription.Text = thisData("Description").ToString()
            lblStatus.Text = thisData("Status").ToString()
            If String.IsNullOrEmpty(lblDescription.Text) Then
                lblDescription.Text = "&nbsp;"
            End If

            Dim promoDetailQuery As String = "SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "' AND (Status='Active' OR Status='Inactive')"
            If Session("RoleName") = "Developer" Then
                promoDetailQuery = "SELECT * FROM PromoDetails WHERE PromoId='" & promoId & "'"
            End If
            gvList.DataSource = settingClass.GetDataTable(promoDetailQuery)
            gvList.DataBind()
            gvList.Columns(1).Visible = LoginAccess("Visible ID Detail")

            btnEdit.Visible = LoginAccess("Edit")
            aDelete.Visible = LoginAccess("Delete")

            btnAddDetail.Visible = LoginAccess("Add Detail")
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function TextStatusDetail(status As String) As String
        Dim result As String = "Activate"
        If status = "Active" Then : Return "Deactivate" : End If
        Return result
    End Function

    Protected Function DiscountTitle(type As String, dataId As String) As String
        If String.IsNullOrEmpty(type) Then Return String.Empty
        If type = "FrameColours" Then Return dataId
        If type = "RollerFabrics" OrElse type = "CurtainFabrics" Then
            Return settingClass.GetItemData(String.Format("SELECT Name FROM Fabrics WHERE Id='{0}'", dataId))
        End If
        If type = "RollerFabricColours" OrElse type = "CurtainFabricColours" Then
            Return settingClass.GetItemData(String.Format("SELECT Name FROM FabricColours WHERE Id='{0}'", dataId))
        End If
        Return settingClass.GetItemData(String.Format("SELECT Name FROM {0} WHERE Id='{1}'", type, dataId))
    End Function

    Protected Function TextStatus(status As String) As String
        Dim result As String = "Activate"
        If status = "Active" Then : Return "Deactivate" : End If
        Return result
    End Function

    Protected Function DiscountValue(method As String, data As Decimal) As String
        If data > 0 Then
            If method = "Percent" Then
                Return data.ToString("G29", enUS) & "%"
            End If
            If method = "Value" Then
                If lblCompanyId.Text = "2" Then
                    Return "$" & data.ToString("G29", enUS)
                End If
                If lblCompanyId.Text = "3" Then
                    Return "Rp" & data.ToString("G29", enUS)
                End If
            End If
        End If
        Return "ERROR"
    End Function

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
