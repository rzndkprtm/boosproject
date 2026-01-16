Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Price_Surcharge_Detail
    Inherits Page

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString

    Dim settingClass As New SettingClass

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("surchargeid")) Then
            Response.Redirect("~/setting/price/surcharge", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("surchargeid").ToString()

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlDesign_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlind(ddlDesign.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesign.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            If ddlBlind.SelectedValue = "" Then
                MessageError(True, "BLIND TYPE IS REQURIED !")
                Exit Sub
            End If

            If ddlBlindNumber.SelectedValue = "" Then
                MessageError(True, "BLIND NUMBER IS REQUIRED !")
                Exit Sub
            End If

            If ddlFieldName.SelectedValue = "" Then
                MessageError(True, "FIELD NAME IS REQUIRED !")
                Exit Sub
            End If

            If ddlPriceGroup.SelectedValue = "" Then
                MessageError(True, "PRICE GROUP IS REQUIRED !")
                Exit Sub
            End If

            If txtFormula.Text = "" Then
                MessageError(True, "FORMULA IS REQUIRED !")
                Exit Sub
            End If

            If txtBuyCharge.Text = "" Then
                MessageError(True, "BUY CHARGE IS REQUIRED !")
                Exit Sub
            End If
            If txtSellCharge.Text = "" Then
                MessageError(True, "SELL CHARGE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim thisId As String = settingClass.CreateId("SELECT TOP 1 Id FROM PriceSurcharges ORDER BY Id DESC")
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")
                Dim finalFormula As String = ddlFieldName.SelectedValue & " " & txtFormula.Text

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE PriceSurcharges SET DesignId=@DesignId, BlindId=@BlindId, BlindNumber=@BlindNumber, PriceGroupId=@PriceGroupId, Name=@Name, FieldName=@FieldName, Formula=@Formula, BuyCharge=@BuyCharge, SellCharge=@SellCharge, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        myCmd.Parameters.AddWithValue("@BlindId", ddlBlind.SelectedValue)
                        myCmd.Parameters.AddWithValue("@PriceGroupId", ddlPriceGroup.SelectedValue)
                        myCmd.Parameters.AddWithValue("@BlindNumber", ddlBlindNumber.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@FieldName", ddlFieldName.SelectedValue)
                        myCmd.Parameters.AddWithValue("@Formula", finalFormula)
                        myCmd.Parameters.AddWithValue("@BuyCharge", txtBuyCharge.Text.Trim())
                        myCmd.Parameters.AddWithValue("@SellCharge", txtSellCharge.Text.Trim())
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"PriceSurcharges", thisId, Session("LoginId").ToString(), "Price Surcharge Created"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/price/surcharge", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/price/surcharge", False)
    End Sub

    Protected Sub BindData(surchargeId As String)
        Try
            Dim myData As DataRow = settingClass.GetDataRow("SELECT * FROM PriceSurcharges WHERE Id='" & surchargeId & "'")

            If myData Is Nothing Then
                Response.Redirect("~/setting/price/surcharge", False)
                Exit Sub
            End If

            Dim designId As String = myData("DesignId").ToString()

            BindDesign()
            BindBlind(designId)
            BindFieldName()
            BindPriceGroup()

            ddlDesign.SelectedValue = designId
            ddlBlind.SelectedValue = myData("BlindId").ToString()
            ddlBlindNumber.SelectedValue = myData("BlindNumber").ToString()
            ddlPriceGroup.SelectedValue = myData("PriceGroupId").ToString()
            txtName.Text = myData("Name").ToString()
            ddlFieldName.SelectedValue = myData("FieldName").ToString()
            Dim formula As String = myData("Formula").ToString()
            If formula.StartsWith(ddlFieldName.SelectedValue) Then
                formula = formula.Substring(ddlFieldName.SelectedValue.Length).TrimStart()
            End If
            txtFormula.Text = formula
            txtBuyCharge.Text = myData("BuyCharge").ToString()
            txtSellCharge.Text = myData("SellCharge").ToString()
            txtDescription.Text = myData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(myData("Active"))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            ddlDesign.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindBlind(designId As String)
        ddlBlind.Items.Clear()
        Try
            If Not designId = "" Then
                ddlBlind.DataSource = settingClass.GetDataTable("SELECT * FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlind.DataTextField = "Name"
                ddlBlind.DataValueField = "Id"
                ddlBlind.DataBind()

                If ddlBlind.Items.Count > 1 Then
                    ddlBlind.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindFieldName()
        ddlFieldName.Items.Clear()
        Try
            ddlFieldName.DataSource = settingClass.GetDataTable("SELECT COLUMN_NAME AS FieldName FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME=N'viewSurcharge'")
            ddlFieldName.DataTextField = "FieldName"
            ddlFieldName.DataValueField = "FieldName"
            ddlFieldName.DataBind()

            If ddlFieldName.Items.Count > 0 Then
                ddlFieldName.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

    Protected Sub BindPriceGroup()
        ddlPriceGroup.Items.Clear()
        Try
            ddlPriceGroup.DataSource = settingClass.GetDataTable("SELECT * FROM PriceGroups ORDER BY Name ASC")
            ddlPriceGroup.DataTextField = "Name"
            ddlPriceGroup.DataValueField = "Id"
            ddlPriceGroup.DataBind()

            ddlPriceGroup.Items.Insert(0, New ListItem("", ""))
        Catch ex As Exception
            MessageError(True, ex.ToString())
        End Try
    End Sub

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
