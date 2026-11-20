Imports System.Data
Imports System.Data.SqlClient

Partial Class Schedule_Edit
    Inherits Page

    Dim scheduleClass As New ShipmentScheduleClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/schedule", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("scheduleid")) Then
            Response.Redirect("~/schedule", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("scheduleid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Dim destinationCity As String = String.Empty
                If ddlAirport.SelectedValue = "SYD" Then
                    destinationCity = "Sydney"
                End If
                If ddlAirport.SelectedValue = "MEL" Then
                    destinationCity = "Melbourne"
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE ShipmentSchedules SET ShipmentNumber=@ShipmentNumber, DestinationCity=@DestinationCity, DestinationAirport=@DestinationAirport, ETD=@ETD, ETA=@ETD WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@DestinationCity", destinationCity)
                        thisCmd.Parameters.AddWithValue("@DestinationAirport", ddlAirport.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@ETD", If(String.IsNullOrEmpty(txtETD.Text), CType(DBNull.Value, Object), txtETD.Text))
                        thisCmd.Parameters.AddWithValue("@ETA", If(String.IsNullOrEmpty(txtETA.Text), CType(DBNull.Value, Object), txtETA.Text))
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"ShipmentSchedules", lblId.Text, Session("LoginId").ToString(), "Shipment Schedule Udpated"}
                scheduleClass.Logs(dataLog)

                Dim url As String = "~/schedule"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/schedule/detail?scheduleid={0}", lblId.Text)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/schedule", False)
    End Sub

    Protected Sub BindData(scheduleId As String)
        Try
            If String.IsNullOrEmpty(scheduleId) Then
                Response.Redirect("~/schedule", False)
                Exit Sub
            End If

            Dim thisQuery As String = "SELECT * FROM ShipmentSchedules WHERE Id='" & scheduleId & "' AND Status='Active'"
            If Session("RoleName") = "Developer" Then
                thisQuery = "SELECT * FROM ShipmentSchedules WHERE Id='" & scheduleId & "'"
            End If

            Dim myData As DataRow = scheduleClass.GetDataRow(thisQuery)
            If myData Is Nothing Then
                Response.Redirect("~/schedule", False)
                Exit Sub
            End If

            txtShipmentNumber.Text = myData("ShipmentNumber").ToString()
            ddlAirport.SelectedValue = myData("DestinationAirport").ToString()
            txtETD.Text = If(IsDBNull(myData("ETD")), String.Empty, Convert.ToDateTime(myData("ETD")).ToString("yyyy-MM-dd"))
            txtETA.Text = If(IsDBNull(myData("ETA")), String.Empty, Convert.ToDateTime(myData("ETA")).ToString("yyyy-MM-dd"))
            txtNote.Text = myData("Notes").ToString()
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
