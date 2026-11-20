Imports System.Data.SqlClient

Partial Class Schedule_Add
    Inherits Page

    Dim scheduleClass As New ShipmentScheduleClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/schedule", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If msgError.InnerText = "" Then
                Dim thisId As String = scheduleClass.CreateShipmentScheduleId()

                Dim destinationCity As String = String.Empty
                If ddlAirport.SelectedValue = "SYD" Then
                    destinationCity = "Sydney"
                End If
                If ddlAirport.SelectedValue = "MEL" Then
                    destinationCity = "Melbourne"
                End If

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO ShipmentSchedules VALUES (@Id, @ShipmentNumber, @ShipmentType, @ShipmentStatus, @OriginCountry, @OriginCity, @OriginAirport, @DestinationCountry, @DestinationCity, @DestinationAirport, @Airline, @FlightNumber, @ETD, @ETA, @BookingReference, @AirwayBillNumber, @TotalPackages, @TotalWeight, @TotalVolume, @Notes, @Status)", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", thisId)
                        thisCmd.Parameters.AddWithValue("@ShipmentNumber", txtShipmentNumber.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@ShipmentType", "Air Freight")
                        thisCmd.Parameters.AddWithValue("@ShipmentStatus", "Draft")
                        thisCmd.Parameters.AddWithValue("@OriginCountry", "Indonesia")
                        thisCmd.Parameters.AddWithValue("@OriginCity", "Jakarta")
                        thisCmd.Parameters.AddWithValue("@OriginAirport", "CGK")
                        thisCmd.Parameters.AddWithValue("@DestinationCountry", "Australia")
                        thisCmd.Parameters.AddWithValue("@DestinationCity", destinationCity)
                        thisCmd.Parameters.AddWithValue("@DestinationAirport", ddlAirport.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Airline", "")
                        thisCmd.Parameters.AddWithValue("@FlightNumber", "")
                        thisCmd.Parameters.AddWithValue("@ETD", If(String.IsNullOrEmpty(txtETD.Text), CType(DBNull.Value, Object), txtETD.Text))
                        thisCmd.Parameters.AddWithValue("@ETA", If(String.IsNullOrEmpty(txtETA.Text), CType(DBNull.Value, Object), txtETA.Text))
                        thisCmd.Parameters.AddWithValue("@BookingReference", "")
                        thisCmd.Parameters.AddWithValue("@AirwayBillNumber", "")
                        thisCmd.Parameters.AddWithValue("@TotalPackages", 0)
                        thisCmd.Parameters.AddWithValue("@TotalWeight", 0)
                        thisCmd.Parameters.AddWithValue("@TotalVolume", 0)
                        thisCmd.Parameters.AddWithValue("@Notes", txtNote.Text)
                        thisCmd.Parameters.AddWithValue("@Status", "Active")

                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"ShipmentSchedules", thisId, Session("LoginId").ToString(), "Shipment Schedule Created"}
                scheduleClass.Logs(dataLog)

                Dim url As String = String.Format("~/schedule/detail?scheduleid={0}", thisId)
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
