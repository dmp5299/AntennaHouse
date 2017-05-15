<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="2.0" xmlns:fo="http://www.w3.org/1999/XSL/Format"
	xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema">
	<xsl:import href="attribute-sets.xsl"/>
	<xsl:import href="tables.xsl"/>
	<xsl:output indent="yes"/>

	<xsl:template match="/">
		<fo:root>
			<fo:layout-master-set>
				<fo:simple-page-master master-name="Main"
						page-width="8.5in" page-height="11in"
						margin-left=".94in" margin-right=".58in" margin-top="1in">
					<fo:region-body margin-top="1in" margin-bottom="1.55in" column-count="1"/>
					<fo:region-before extent="2cm" region-name="Main.Header"/>
					<fo:region-after extent="1.48in" region-name="Main.Footer"/>
					<fo:region-start />
					<fo:region-end/>
				</fo:simple-page-master>
			</fo:layout-master-set>
			<xsl:apply-templates select="dmodule"/>
		</fo:root>
	</xsl:template>
	
	<xsl:template match="dmodule">
		<fo:page-sequence master-reference="Main" initial-page-number="1">
			<xsl:call-template name="static-content"/>
			<fo:flow flow-name="xsl-region-body">
				<fo:block>
					<xsl:apply-templates select="descendant::table"/>
				</fo:block>
			</fo:flow>
		</fo:page-sequence>
	</xsl:template>
	
	<xsl:template name="title-page">
		<fo:block xsl:use-attribute-sets="normal-text" text-align="center">
			<fo:block xsl:use-attribute-sets="bold-normal-text" font-size="14pt">
				<xsl:apply-templates select="descendant::techName[1]"/>
			</fo:block>
			PROPRIETARY<fo:block padding-after=".3in"/>
			<xsl:apply-templates select="descendant::dataDistribution[1]"/><fo:block padding-after=".3in"/>
			<fo:block xsl:use-attribute-sets="bold-normal-text" font-size="14pt" padding-after=".3in">
				<xsl:apply-templates select="descendant::simplePara[1]"/><fo:block padding-after=".5in"/>
				Management Information<fo:block padding-after=".3in"/>
				Compliance Category
			</fo:block>
			4
			<fo:block xsl:use-attribute-sets="bold-normal-text" font-size="14pt" padding-after=".3in">
				Original Issue Date
			</fo:block>
			<xsl:for-each select="descendant::issueDate[1]">
				<xsl:value-of select="concat(@month,'-',@day,'-',@year)"/>
			</xsl:for-each>
		</fo:block>
	</xsl:template>
	
	<xsl:template match="techName|dataDistribution|simplePara"><xsl:apply-templates/></xsl:template>
	
	<xsl:template name="static-content">
		<fo:static-content flow-name="Main.Header">
		<fo:block>
			<fo:block font-family="sans-serif" font-size="14pt" font-weight="bold" text-align="center">
				Hamilton Sundstrand Corporation, a UTC Aerospace Systems Company
			</fo:block>
			<fo:block font-family="sans-serif" font-size="22pt" font-weight="bold" text-align="center">
				SERVICE BULLETIN
			</fo:block>
			<fo:leader leader-length="100%" leader-pattern="rule"/>
		</fo:block>
		</fo:static-content>
		<fo:static-content flow-name="Main.Footer">
		<fo:block>
			blah
		</fo:block>
		</fo:static-content>
	</xsl:template>
	
	<xsl:template match="content"/>
	
	<xsl:template match="identAndStatusSection">
		
	</xsl:template>
	
</xsl:stylesheet>