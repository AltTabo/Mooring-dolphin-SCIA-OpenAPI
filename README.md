# Mooring-dolphin-SCIA-OpenAPI

Este programa realiza a modelagem e a análise estrutural de uma ou mais estruturas de dolphins de amarração, utilizando o OpenAPI do SCIA Engineer. 

Para utilizar o programa será necessário adaptar os seguintes campos:


Linhas 51 e 52:

            Variaveis variaveis = new Variaveis(6, 5, 1.5, 75, 45, 7.7, 21.3, 20, 9.7, 1.5, 1, 4.57e6, 75.405e6, 1.5, 2.5, 5e3, 320, 320, 10e3);
            Variaveis_iter var_iter = new Variaveis_iter(10, 15, 0, 0, 1, 1, 1, 1);

![image](https://github.com/AltTabo/Mooring-dolphin-SCIA-OpenAPI/assets/141842536/e2c0abb0-7348-40e7-b3a6-2bcd28dcee2a)

![image](https://github.com/AltTabo/Mooring-dolphin-SCIA-OpenAPI/assets/141842536/079fd37c-95e1-45a7-b658-3c744066271e)


Para a modelagem, são definidos os seguintes parâmetros:



13 parâmetros geométricos do modelo:

a:	Largura da laje em metros, apresentada no eixo X

b:	Comprimento da laje em metros, apresentada no eixo Y

c:	Espessura da laje em metros, apresentada no eixo Z

α:	Ângulo de inclinação da estaca com relação à sua projeção no plano XY

β:	Ângulo de rotação da projeção da estaca no plano XY em relação ao eixo X

Cab_x:	Distância do eixo central de cada cabeço em relação às bordas paralelas ao eixo Y

Cab_y:	Distância do eixo central de cada cabeço em relação ao eixo X (b/2 ≤ Cab_y < b)

Est_x:	Distância do centro da estaca no plano XY em relação às bordas paralelas ao eixo X

Est_y:	Distância do centro da estaca no plano XY em relação às bordas paralelas ao eixo Y

hsolo:	Profundidade de solo em que a extremidade da estaca se apoia

hlaje_solo:	Altura da laje (plano XY) com relação ao solo

hnível_água:	Altura máxima do nível da agua (utilizado para o cálculo das forças de corrente)

hconcretada:	Altura da estaca a ser concretada


Parâmetros geométricos do modelo, vista superior

![image](https://github.com/AltTabo/Mooring-dolphin-SCIA-OpenAPI/assets/141842536/46d3ba20-6657-40d9-9932-7e0f31e26068)

Parâmetros geométricos do modelo, vista lateral

![image](https://github.com/AltTabo/Mooring-dolphin-SCIA-OpenAPI/assets/141842536/0d64e376-a6b4-4464-b6d3-67e6bf601bcb)

Parâmetros de altura do modelo

![image](https://github.com/AltTabo/Mooring-dolphin-SCIA-OpenAPI/assets/141842536/128a3b07-71d8-4be1-a65c-fc5217b7e52c)

Ângulo α, corte vertical passando pela projeção da estaca no plano XY

![image](https://github.com/AltTabo/Mooring-dolphin-SCIA-OpenAPI/assets/141842536/a2ee55b5-5fca-438d-b240-63c96c371497)

6 outros parâmetros do modelo:

Rigidezinicial:	Valor inicial da rigidez do solo, em N/m

Rigidezfinal:	Valor final da rigidez do solo, em N/m

Sobrecarga:	Valor da ação de sobrecarga, em N/m² 

ForçaCorrente_transv:	Valor da ação de corrente na direção transversal (paralela ao eixo X), em N

ForçaCorrente_long:	Valor da ação de corrente na direção longitudinal (paralela ao eixo Y), em N

ForçaCabeço:	Valor da ação de amarração do navio, em N





