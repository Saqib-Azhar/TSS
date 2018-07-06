#INCLUDE \VFE7\VFEFRAME\INCLUDE\VFE.H


FUNCTION ConvertTimeSlipImport_Jupiter847
*==============================================================================
* Purpose:		Creates a TimeSlip Import compaitable file from another.
*
* Author:		Complete Computer Service, Ltd.
*
* Returns:		character: blank if successful, otherwise an error message
*==============================================================================
LPARAMETERS ;
	toReport AS "TimeSlipUtilities" OF LIBS\ReportObjects.VCX, ;
	tcSourceFile AS CHARACTER, ;
	tcImportFile AS CHARACTER

LOCAL ;
	lcReturnMsg AS CHARACTER, ;
	lcAlsImport AS CHARACTER, ;
	loSelect AS "cSelect" OF \VFE7\VFEFRAME\LIBS\cUtils.VCX, ;
	lnFhSource AS INTEGER, ;
	lcRowSource AS CHARACTER, ;
	lcFirstName AS CHARACTER, ;
	lcRateCode AS CHARACTER

* setup the local variables
STORE "" TO lcReturnMsg

* generate an alias for the import cursor
lcAlsImport = GetTempFileName()

* preserve the current alias
loSelect = CREATEOBJECT("cSelect")


DO CASE
	* validate the parameters
	CASE (VARTYPE(tcSourceFile) <> T_CHARACTER) OR EMPTY(tcSourceFile)
		lcReturnMsg = "Invalid Parameter: tcSourceFile"
		
	* validate the parameters
	CASE (VARTYPE(tcImportFile) <> T_CHARACTER) OR EMPTY(tcImportFile)
		lcReturnMsg = "Invalid Parameter: tcImportFile"
	
	* check if the source file exists
	CASE FileExists(tcSourceFile) = .F.
		lcReturnMsg = "The '" + tcSourceFile + "' file does not exist."
	
	OTHERWISE
		* attempt to open the source file
		lnFhSource = FOPEN(tcSourceFile, F_READONLY)
		IF lnFhSource = -1
			lcReturnMsg = "The '" + tcSourceFile + "' file could not be opened."
		ELSE
			* create the import cursor
			CREATE CURSOR (lcAlsImport) (;
				cSlipId C(6), ;
				cJobOrderId C(10), ;
				cCustomerId C(10), ;
				cCustomerName C(10), ;
				cSiteCode C(6), ;
				cEmployeeId C(10), ;
				cEmployeeName C(10), ;
				cRateCode C(10), ;
				cWeekend C(10), ;
				cBatch C(10), ;
				cHourType C(2), ;
				nRegPayHours N(8,2), ;
				nRegPayRate N(10,4), ;
				nRegBillHours N(8,2), ;
				nRegBillRate N(10,4), ;
				nOvtPayHours N(8,2), ;
				nOvtPayRate N(10,4), ;
				nOvtBillHours N(8,2), ;
				nOvtBillRate N(10,4), ;
				nDblPayHours N(8,2), ;
				nDblPayRate N(10,4), ;
				nDblBillHours N(8,2), ;
				nDblBillRate N(10,4), ;
				cCompCode C(6), ;
				cSalesTaxCode C(6), ;
				cPONumber C(15), ;
				cRelease C(15), ;
				cProject C(15), ;
				cDepartmentCode C(3), ;
				cOfficeCode C(3), ;
				cLocationCode C(3), ;
				cSalesman1Code C(3), ;
				cSalesman2Code C(3), ;
				cPayFrequency C(1), ;
				nNumberOfDays N(3), ;
				cPayHold C(1), ;
				cBillHold C(1), ;
				cSeparateCheck C(1), ;
				MiscPayAmount1 N(8,2), ;
				MiscBillAmount1 N(8,2), ;
				MiscPayAmount2 N(8,2), ;
				MiscBillAmount2 N(8,2), ;
				MiscPayAmount3 N(8,2), ;
				MiscBillAmount3 N(8,2), ;
				MiscPayAmount4 N(8,2), ;
				MiscBillAmount4 N(8,2), ;
				MiscPayAmount5 N(8,2), ;
				MiscBillAmount5 N(8,2), ;
				MiscPayAmount6 N(8,2), ;
				MiscBillAmount6 N(8,2), ;
				MiscPayAmount7 N(8,2), ;
				MiscBillAmount7 N(8,2), ;
				MiscPayAmount8 N(8,2), ;
				MiscBillAmount8 N(8,2), ;
				MiscPayAmount9 N(8,2), ;
				MiscBillAmount9 N(8,2) )
			
			* move the cursor to begining of the source file
			FSEEK(lnFhSource, 0, 0)  && BOF

			* begin processing each row of the source file
			DO WHILE !FEOF(lnFhSource)

				* get a row from the source file
				lcRowSource = FGETS(lnFhSource, 8192)

				lcFirstName = SUBSTR(lcRowSource, 87, 16)
				lcRateCode = IIF(OCCURS("-", lcFirstName)>0, SUBSTR(lcFirstName, RAT("-", lcFirstName)+1), "")
								
				* update the import cursor
				INSERT INTO (lcAlsImport) (;
					cCustomerId, ;
					cEmployeeId, ;
					cEmployeeName, ;
					cWeekEnd, ;
					nRegPayHours, ;
					nRegBillHours, ;
					nOvtPayHours, ;
					nOvtBillHours, ;
					cRateCode ;
					) ;
				VALUES (;
					SUBSTR(lcRowSource, 17, 15), ;
					SUBSTR(lcRowSource, 47, 15), ;
					SUBSTR(lcRowSource, 62, 25), ;
					DateToChar(CharToDate(SUBSTR(lcRowSource, 9, 8), 1), "", 1), ;
					VAL(SUBSTR(lcRowSource, 166, 11)), ;
					VAL(SUBSTR(lcRowSource, 166, 11)), ;
					VAL(SUBSTR(lcRowSource, 177, 11)), ;
					VAL(SUBSTR(lcRowSource, 177, 11)), ;
					lcRateCode ;
					)

			ENDDO
			
			* close the source file
			FCLOSE(lnFhSource)
			
			* attempt to create the import file
			SELECT (lcAlsImport)
			EXPORT TO (tcImportFile) TYPE XL5
			
			* close the import cursor
			USE IN (lcAlsImport)
		ENDIF
		
ENDCASE


* restore the previous alias
loSelect.RELEASE()
STORE .NULL. TO loSelect


RETURN lcReturnMsg



FUNCTION ConvertTimeSlipImport_Jupiter900
*==============================================================================
* Purpose:		Creates a TimeSlip Import compaitable file from another.
*
* Author:		Complete Computer Service, Ltd.
*
* Returns:		character: blank if successful, otherwise an error message
*==============================================================================
LPARAMETERS ;
	toReport AS "TimeSlipUtilities" OF LIBS\ReportObjects.VCX, ;
	tcSourceFile AS CHARACTER, ;
	tcImportFile AS CHARACTER

LOCAL ;
	lcReturnMsg AS CHARACTER, ;
	lcAlsImport AS CHARACTER, ;
	loSelect AS "cSelect" OF \VFE7\VFEFRAME\LIBS\cUtils.VCX, ;
	lnFhSource AS INTEGER, ;
	lcRowSource AS CHARACTER, ;
	lcFirstName AS CHARACTER, ;
	lcRateCode AS CHARACTER

* setup the local variables
STORE "" TO lcReturnMsg

* generate an alias for the import cursor
lcAlsImport = GetTempFileName()

* preserve the current alias
loSelect = CREATEOBJECT("cSelect")


DO CASE
	* validate the parameters
	CASE (VARTYPE(tcSourceFile) <> T_CHARACTER) OR EMPTY(tcSourceFile)
		lcReturnMsg = "Invalid Parameter: tcSourceFile"
		
	* validate the parameters
	CASE (VARTYPE(tcImportFile) <> T_CHARACTER) OR EMPTY(tcImportFile)
		lcReturnMsg = "Invalid Parameter: tcImportFile"
	
	* check if the source file exists
	CASE FileExists(tcSourceFile) = .F.
		lcReturnMsg = "The '" + tcSourceFile + "' file does not exist."
	
	OTHERWISE
		* attempt to open the source file
		lnFhSource = FOPEN(tcSourceFile, F_READONLY)
		IF lnFhSource = -1
			lcReturnMsg = "The '" + tcSourceFile + "' file could not be opened."
		ELSE
			* create the import cursor
			CREATE CURSOR (lcAlsImport) (;
				cSlipId C(6), ;
				cJobOrderId C(10), ;
				cCustomerId C(10), ;
				cCustomerName C(10), ;
				cSiteCode C(6), ;
				cEmployeeId C(10), ;
				cEmployeeName C(10), ;
				cRateCode C(10), ;
				cWeekend C(10), ;
				cBatch C(10), ;
				cHourType C(2), ;
				nRegPayHours N(8,2), ;
				nRegPayRate N(10,4), ;
				nRegBillHours N(8,2), ;
				nRegBillRate N(10,4), ;
				nOvtPayHours N(8,2), ;
				nOvtPayRate N(10,4), ;
				nOvtBillHours N(8,2), ;
				nOvtBillRate N(10,4), ;
				nDblPayHours N(8,2), ;
				nDblPayRate N(10,4), ;
				nDblBillHours N(8,2), ;
				nDblBillRate N(10,4), ;
				cCompCode C(6), ;
				cSalesTaxCode C(6), ;
				cPONumber C(15), ;
				cRelease C(15), ;
				cProject C(15), ;
				cDepartmentCode C(3), ;
				cOfficeCode C(3), ;
				cLocationCode C(3), ;
				cSalesman1Code C(3), ;
				cSalesman2Code C(3), ;
				cPayFrequency C(1), ;
				nNumberOfDays N(3), ;
				cPayHold C(1), ;
				cBillHold C(1), ;
				cSeparateCheck C(1), ;
				MiscPayAmount1 N(8,2), ;
				MiscBillAmount1 N(8,2), ;
				MiscPayAmount2 N(8,2), ;
				MiscBillAmount2 N(8,2), ;
				MiscPayAmount3 N(8,2), ;
				MiscBillAmount3 N(8,2), ;
				MiscPayAmount4 N(8,2), ;
				MiscBillAmount4 N(8,2), ;
				MiscPayAmount5 N(8,2), ;
				MiscBillAmount5 N(8,2), ;
				MiscPayAmount6 N(8,2), ;
				MiscBillAmount6 N(8,2), ;
				MiscPayAmount7 N(8,2), ;
				MiscBillAmount7 N(8,2), ;
				MiscPayAmount8 N(8,2), ;
				MiscBillAmount8 N(8,2), ;
				MiscPayAmount9 N(8,2), ;
				MiscBillAmount9 N(8,2) )
			
			* move the cursor to begining of the source file
			FSEEK(lnFhSource, 0, 0)  && BOF

			* begin processing each row of the source file
			DO WHILE !FEOF(lnFhSource)

				* get a row from the source file
				lcRowSource = FGETS(lnFhSource, 8192)

				lcFirstName = SUBSTR(lcRowSource, 79, 30)
				lcRateCode = IIF(OCCURS("-", lcFirstName)>0, SUBSTR(lcFirstName, RAT("-", lcFirstName)+1), "")
				
				* update the import cursor
				INSERT INTO (lcAlsImport) (;
					cEmployeeId, ;
					cEmployeeName, ;
					cWeekEnd, ;
					nRegPayHours, ;
					nRegBillHours, ;
					nOvtPayHours, ;
					nOvtBillHours, ;
					cRateCode ;
					) ;
				VALUES (;
					SUBSTR(lcRowSource, 33, 6), ;
					SUBSTR(lcRowSource, 54, 25), ;
					DateToChar(CharToDate(SUBSTR(lcRowSource, 9, 8), 1), "", 1), ;
					VAL(SUBSTR(lcRowSource, 121, 9)), ;
					VAL(SUBSTR(lcRowSource, 121, 9)), ;
					VAL(SUBSTR(lcRowSource, 130, 9)), ;
					VAL(SUBSTR(lcRowSource, 130, 9)), ;
					lcRateCode ;
					)

			ENDDO
			
			* close the source file
			FCLOSE(lnFhSource)
			
			* attempt to create the import file
			SELECT (lcAlsImport)
			EXPORT TO (tcImportFile) TYPE XL5
			
			* close the import cursor
			USE IN (lcAlsImport)
		ENDIF
		
ENDCASE


* restore the previous alias
loSelect.RELEASE()
STORE .NULL. TO loSelect


RETURN lcReturnMsg
