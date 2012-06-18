<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
    <xsl:output method="html"/>
    <xsl:template match="/">
        <html>
            <head>
                <title>Poll export</title>
            </head>
            <body>
                <h2>
                    <xsl:value-of select="pollName"/>
                </h2>

                <ul>
                    <xsl:for-each select="votingOptions/votingOption">
                        <li>
                            <xsl:value-of select="name"/>
                            <xsl:value-of select="votes"/>
                        </li>
                    </xsl:for-each>
                </ul>
            </body>
        </html>
    </xsl:template>
</xsl:stylesheet>