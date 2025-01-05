# Summary

| Entry              | Value |
|              ----: | :---- |
| Execution Start    | 2025-01-05 20:26:20 UTC |
| Execution Duration | 6s 594ms |
| **Overall Status** | :red_circle: Failed |
| Total Features     | 14 |
| Total Scenarios    | 34 |
| Passed Scenarios   | 18 |
| Bypassed Scenarios | 1 |
| Failed Scenarios   | 11 |
| Ignored Scenarios  | 4 |
| Total Steps        | 167 |
| Passed Steps       | 134 |
| Bypassed Steps     | 3 |
| Failed Steps       | 17 |
| Ignored Steps      | 8 |
| Not Run Steps      | 5 |

# Features


## Address book feature
> In order to maintain my product dispatch
> As an application user
> I want to add and browse my client postal addresses by client emails


### :red_circle: Scenario: Adding contacts :watch:`50ms`
> This scenario presents failures captured by VerifiableTree

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN an empty address book :watch:`1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I associate contact "`<$contact>`" with address "`<$address>`" as alias "`Joey`" :watch:`25ms`
**$contact**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Email` | `joe67@email.com` |
| `$.Name` | `Joe Jonnes` |
| `$.PhoneNumber` | `666777888` |

</div>

**$address**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Address` | `47 Main Street` |
| `$.City` | `London` |
| `$.Country` | `UK` |
| `$.PostCode` | `AB1 2CD` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND I associate contact "`<$contact>`" with address "`<$address>`" as alias "`Janek`" :watch:`<1ms`
**$contact**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Email` | `nowak33@email.com` |
| `$.Name` | `Jan Nowak` |
| `$.PhoneNumber` | `123654789` |

</div>

**$address**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Address` | `Rynek Główny 1` |
| `$.City` | `Kraków` |
| `$.Country` | `Poland` |
| `$.PostCode` | `31-042` |

</div>

#### &nbsp;&nbsp;&nbsp; :red_circle: Step 4: THEN address book should contain contacts "`<$contacts>`❗" :watch:`5ms`
**$contacts**:
<div style="overflow-x: auto;">

| # | Node | Value |
|---| ---- | ----- |
| ☑ | `$` | `<array:2>` |
| ☑ | `$[0]` | `<object>` |
| ☑ | `$[0].Email` | `nowak33@email.com` |
| ☑ | `$[0].Name` | `Jan Nowak` |
| ☑ | `$[0].PhoneNumber` | `123654789` |
| ☑ | `$[1]` | `<object>` |
| ☑ | `$[1].Email` | `joe67@email.com` |
| ❗ | `$[1].Name` | `Joel Jonnes` / `Joe Jonnes` |
| ☑ | `$[1].PhoneNumber` | `666777888` |

</div>

> [!IMPORTANT]
> <pre>
> Step 4: System.InvalidOperationException : Parameter 'contacts' verification failed: $[1].Name: expected: equals 'Joel Jonnes', but got: 'Joe Jonnes'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


### :white_check_mark: Scenario: Matching addresses by email :watch:`7ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN an empty address book :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I associate contact "`<$contact>`" with address "`<$address>`" as alias "`Joey`" :watch:`<1ms`
**$contact**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Email` | `joe67@email.com` |
| `$.Name` | `Joe Jonnes` |
| `$.PhoneNumber` | `666777888` |

</div>

**$address**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Address` | `47 Main Street` |
| `$.City` | `London` |
| `$.Country` | `UK` |
| `$.PostCode` | `AB1 2CD` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND I associate contact "`<$contact>`" with address "`<$address>`" as alias "`Janek`" :watch:`<1ms`
**$contact**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Email` | `nowak33@email.com` |
| `$.Name` | `Jan Nowak` |
| `$.PhoneNumber` | `123654789` |

</div>

**$address**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Address` | `Rynek Główny 1` |
| `$.City` | `Kraków` |
| `$.Country` | `Poland` |
| `$.PostCode` | `31-042` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: THEN address by email should match "`<$match>`☑" :watch:`2ms`
**$match**:
<div style="overflow-x: auto;">

| # | Node | Value |
|---| ---- | ----- |
| ☑ | `$` | `<object>` |
| ☑ | `$['joe67@email.com']` | `<object>` |
| ☑ | `$['joe67@email.com'].Address` | `<object>` |
| ☑ | `$['joe67@email.com'].Address.Address` | `47 Main Street` |
| ☑ | `$['joe67@email.com'].Address.City` | `London` |
| ☑ | `$['joe67@email.com'].Address.PostCode` | `AB1 2CD` |
| ☑ | `$['nowak33@email.com']` | `<object>` |
| ☑ | `$['nowak33@email.com'].Address` | `<object>` |
| ☑ | `$['nowak33@email.com'].Address.Address` | `Rynek Główny 1` |
| ☑ | `$['nowak33@email.com'].Address.City` | `Kraków` |
| ☑ | `$['nowak33@email.com'].Address.PostCode` | `31-042` |

</div>

---


### :white_check_mark: Scenario: Persisting address book :watch:`31ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN an address book with contacts "`<$contacts>`" :watch:`<1ms`
**$contacts**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<array:2>` |
| `$[0]` | `<object>` |
| `$[0].Address` | `<object>` |
| `$[0].Address.Address` | `47 Main Street` |
| `$[0].Address.City` | `London` |
| `$[0].Address.Country` | `UK` |
| `$[0].Address.PostCode` | `AB1 2CD` |
| `$[0].Alias` | `Joey` |
| `$[0].Contact` | `<object>` |
| `$[0].Contact.Email` | `joe67@email.com` |
| `$[0].Contact.Name` | `Joe Jonnes` |
| `$[0].Contact.PhoneNumber` | `666777888` |
| `$[1]` | `<object>` |
| `$[1].Address` | `<object>` |
| `$[1].Address.Address` | `Rynek Główny 1` |
| `$[1].Address.City` | `Kraków` |
| `$[1].Address.Country` | `Poland` |
| `$[1].Address.PostCode` | `31-042` |
| `$[1].Alias` | `Janek` |
| `$[1].Contact` | `<object>` |
| `$[1].Contact.Email` | `nowak33@email.com` |
| `$[1].Contact.Name` | `Jan Nowak` |
| `$[1].Contact.PhoneNumber` | `123654789` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I persist book as json :watch:`20ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN address book should match persisted json "`<$json>`☑" :watch:`5ms`
**$json**:
<div style="overflow-x: auto;">

| # | Node | Value |
|---| ---- | ----- |
| ☑ | `$` | `<object>` |
| ☑ | `$.ContactsByEmail` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com']` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.Address` | `47 Main Street` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.City` | `London` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.Country` | `UK` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.PostCode` | `AB1 2CD` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Alias` | `Joey` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact.Email` | `joe67@email.com` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact.Name` | `Joe Jonnes` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact.PhoneNumber` | `666777888` |
| ☑ | `$.ContactsByEmail['nowak33@email.com']` | `<object>` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address` | `<object>` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.Address` | `Rynek Główny 1` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.City` | `Kraków` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.Country` | `Poland` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.PostCode` | `31-042` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Alias` | `Janek` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact` | `<object>` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact.Email` | `nowak33@email.com` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact.Name` | `Jan Nowak` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact.PhoneNumber` | `123654789` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND address book should match persisted json "`<$json>`☑" :watch:`2ms`
**$json**:
<div style="overflow-x: auto;">

| # | Node | Value |
|---| ---- | ----- |
| ☑ | `$` | `<object>` |
| ☑ | `$.ContactsByEmail` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com']` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.Address` | `47 Main Street` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.City` | `London` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.Country` | `UK` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Address.PostCode` | `AB1 2CD` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Alias` | `Joey` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact` | `<object>` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact.Email` | `joe67@email.com` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact.Name` | `Joe Jonnes` |
| ☑ | `$.ContactsByEmail['joe67@email.com'].Contact.PhoneNumber` | `666777888` |
| ☑ | `$.ContactsByEmail['nowak33@email.com']` | `<object>` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address` | `<object>` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.Address` | `Rynek Główny 1` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.City` | `Kraków` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.Country` | `Poland` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Address.PostCode` | `31-042` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Alias` | `Janek` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact` | `<object>` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact.Email` | `nowak33@email.com` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact.Name` | `Jan Nowak` |
| ☑ | `$.ContactsByEmail['nowak33@email.com'].Contact.PhoneNumber` | `123654789` |

</div>

> [!NOTE]
> <pre>
> Step 3: Underlying type: JsonElement
> Step 4: Underlying type: ExpandoObject
> </pre>

---


### :white_check_mark: Scenario: Retrieving postal addresses :watch:`5ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN an empty address book :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I associate contact "`<$contact>`" with address "`<$address>`" as alias "`Joey`" :watch:`<1ms`
**$contact**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Email` | `joe67@email.com` |
| `$.Name` | `Joe Jonnes` |
| `$.PhoneNumber` | `666777888` |

</div>

**$address**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Address` | `47 Main Street` |
| `$.City` | `London` |
| `$.Country` | `UK` |
| `$.PostCode` | `AB1 2CD` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND I associate contact "`<$contact>`" with address "`<$address>`" as alias "`Janek`" :watch:`<1ms`
**$contact**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Email` | `nowak33@email.com` |
| `$.Name` | `Jan Nowak` |
| `$.PhoneNumber` | `123654789` |

</div>

**$address**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<object>` |
| `$.Address` | `Rynek Główny 1` |
| `$.City` | `Kraków` |
| `$.Country` | `Poland` |
| `$.PostCode` | `31-042` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: THEN address book should contain postal addresses "`<$addresses>`☑" :watch:`1ms`
**$addresses**:
<div style="overflow-x: auto;">

| # | Node | Value |
|---| ---- | ----- |
| ☑ | `$` | `<array:2>` |
| ☑ | `$[0]` | `<object>` |
| ☑ | `$[0].Address` | `Rynek Główny 1` |
| ☑ | `$[0].City` | `Kraków` |
| ☑ | `$[0].Country` | `Poland` |
| ☑ | `$[0].PostCode` | `31-042` |
| ☑ | `$[1]` | `<object>` |
| ☑ | `$[1].Address` | `47 Main Street` |
| ☑ | `$[1].City` | `London` |
| ☑ | `$[1].Country` | `UK` |
| ☑ | `$[1].PostCode` | `AB1 2CD` |

</div>

---


## Basket feature :label:`Story-4`
> In order to buy products
> As a customer
> I want to add products to basket


### :white_check_mark: Scenario: No product in stock :label:`Ticket-6` :file_folder:`Sales` :watch:`1s 527ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN product is out of stock :watch:`7ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN customer adds it to the basket :watch:`1s 508ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN the product addition should be unsuccessful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND the basket should not contain the product :watch:`<1ms`
> [!NOTE]
> <pre>
> Step 2: Transferring 'product' to the basket
> </pre>

---


### :warning: Scenario: Successful addition :label:`Ticket-7` :file_folder:`Sales` :watch:`1s 113ms`
> This scenario presents how LightBDD reports ignored steps

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN product is in stock :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN customer adds it to the basket :watch:`1s 112ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN the product addition should be successful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND the basket should contain the product :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :warning: Step 5: AND the product should be removed from stock :watch:`<1ms`
> [!IMPORTANT]
> <pre>
> Step 5: Product removal from stock is not implemented yet
> </pre>

> [!NOTE]
> <pre>
> Step 1: Added 'product' to the stock
> Step 2: Transferring 'product' to the basket
> </pre>

---


## Calculator feature :label:`Story-8`
> In order to perform calculations correctly
> As a office assistant
> I want to use calculator for my calculations
> 
> This example presents usage of MultiAssertAttribute.


### :red_circle: Scenario: Adding numbers :label:`Ticket-13` :watch:`51ms`
> This scenario presents usage of Verifiable<T> and MultiAssertAttribute

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a calculator :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: THEN adding "`2`" to "`3`" should give "`5`☑" :watch:`11ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 3: AND adding "`-3`" to "`2`" should give "`expected: equals '-1', but got: '1'`❗" :watch:`21ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND adding "`0`" to "`1`" should give "`1`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 5: AND adding "`-2`" to "`-1`" should give "`expected: equals '-3', but got: '3'`❗" :watch:`<1ms`
> [!IMPORTANT]
> <pre>
> Step 3: System.InvalidOperationException : Parameter 'result' verification failed: expected: equals '-1', but got: '1'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> Step 5: System.InvalidOperationException : Parameter 'result' verification failed: expected: equals '-3', but got: '3'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


### :red_circle: Scenario: Composite operations :label:`Ticket-13` :watch:`13ms`
> This scenario presents behavior of MultiAssertAttribute

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a calculator :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 2: THEN it should add numbers :watch:`4ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 2.1: THEN adding "`2`" to "`3`" should give "`5`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :red_circle: Step 2.2: AND adding "`2`" to "`-3`" should give "`expected: equals '-1', but got: '1'`❗" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :red_circle: Step 2.3: AND adding "`0`" to "`1`" should give "`expected: equals '0', but got: '1'`❗" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :warning: Step 3: AND it should multiply numbers :watch:`3ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 3.1: THEN multiplying "`2`" by "`3`" should give "`6`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :warning: Step 3.2: AND multiplying "`2`" by "`-3`" should give "`expected: equals '-6'`⚠" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_circle: Step 3.3: AND multiplying "`1`" by "`1`" should give "`<?>`"
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 4: AND it should divide numbers :watch:`2ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 4.1: THEN dividing "`6`" by "`3`" should give "`2`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :red_circle: Step 4.2: AND multiplying "`5`" by "`2`" should give "`expected: equals '2', but got: '10'`❗" :watch:`<1ms`
> [!IMPORTANT]
> <pre>
> Step 2.2: System.InvalidOperationException : Parameter 'result' verification failed: expected: equals '-1', but got: '1'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> Step 2.3: System.InvalidOperationException : Parameter 'result' verification failed: expected: equals '0', but got: '1'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> Step 3.2: Negative numbers are not supported yet
> Step 4.2: System.InvalidOperationException : Parameter 'result' verification failed: expected: equals '2', but got: '10'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

> [!NOTE]
> <pre>
> Step 2: It is possible to add MultiAssertAttribute on composite step
> Step 3: This step does not have MultiAssertAttribute so will stop on first exception
> </pre>

---


### :red_circle: Scenario: Dividing numbers :label:`Ticket-13` :watch:`5ms`
> This scenario presents usage of Verifiable<T> and MultiAssertAttribute

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a calculator :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: THEN dividing "`6`" by "`2`" should give "`3`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 3: AND dividing "`2`" by "`0`" should give "`expected: equals '0', but got: '<DivideByZeroException>'`❗" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND dividing "`2`" by "`3`" should give "`0`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 5: AND dividing "`0`" by "`5`" should give "`expected: equals '1', but got: '0'`❗" :watch:`<1ms`
> [!IMPORTANT]
> <pre>
> Step 3: System.InvalidOperationException : Parameter 'result' verification failed: expected: equals '0', but got: '<DivideByZeroException>'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> Step 5: System.InvalidOperationException : Parameter 'result' verification failed: expected: equals '1', but got: '0'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


### :warning: Scenario: Multiplying numbers :label:`Ticket-13` :watch:`5ms`
> This scenario presents how steps are ignored when MultiAssertAttribute is applied

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a calculator :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: THEN multiplying "`6`" by "`2`" should give "`12`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :warning: Step 3: AND multiplying "`-1`" by "`2`" should give "`expected: equals '-2'`⚠" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND multiplying "`2`" by "`0`" should give "`0`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: AND multiplying "`2`" by "`3`" should give "`6`☑" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :warning: Step 6: AND multiplying "`2`" by "`-3`" should give "`expected: equals '-6'`⚠" :watch:`<1ms`
> [!IMPORTANT]
> <pre>
> Step 3: Negative numbers are not supported yet
> Step 6: Negative numbers are not supported yet
> </pre>

---


## Compact calculator scenarios
> As LightBDD user,
> I want to be able to write compact scenarios,
> So that I can use LightBDD for more unit-test like tests as well


### :white_check_mark: Scenario: Adding numbers :watch:`32ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN calculator :watch:`7ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I add two numbers :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN I should get an expected result :watch:`14ms`
---


## Contacts management :label:`Story-6`
> In order to maintain my contact book
> As an application user
> I want to add, browse and remove my contacts


### :white_check_mark: Scenario: Contact book should allow me to add multiple contacts :label:`Ticket-8` :watch:`35ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN my contact book is empty :watch:`4ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I add new contacts :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN all contacts should be available in the contact book :watch:`16ms`
---


### :large_blue_diamond: Scenario: Contact book should allow me to remove all contacts :label:`Ticket-9` :watch:`4ms`
> This scenario presents how LightBDD reports bypassed steps

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN my contact book is filled with many contacts :watch:`1ms`
#### &nbsp;&nbsp;&nbsp; :large_blue_diamond: Step 2: WHEN I clear it :watch:`1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN the contact book should be empty :watch:`<1ms`
> [!IMPORTANT]
> <pre>
> Step 2: Contact book clearing is not implemented yet. Contacts are removed one by one.
> </pre>

---


### :white_check_mark: Scenario: Contact book should allow me to remove contacts :label:`Ticket-9` :watch:`3ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN my contact book is filled with contacts :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I remove one contact :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN the contact book should not contain removed contact any more :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND the contact book should contains all other contacts :watch:`<1ms`
---


### :white_check_mark: Scenario: Displaying contacts alphabetically :watch:`39ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN my contact book is empty :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND I added contacts "`<$contacts>`" :watch:`21ms`
**$contacts**:
<div style="overflow-x: auto;">

|Email              |Name   |PhoneNumber   |
|-------------------|-------|--------------|
|`john123@gmail.com`|`John` |`111-222-333` |
|`greg22@gmail.com` |`Greg` |`213-444-444` |
|`emily1@gmail.com` |`Emily`|`111-222-5556`|
|`ka321@gmail.com`  |`Kathy`|`111-555-330` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: WHEN I request contacts sorted by name :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: THEN I should receive contacts "`<$contacts>`☑" :watch:`12ms`
**$contacts**:
<div style="overflow-x: auto;">

|#|Email              |Name   |PhoneNumber   |
|-|-------------------|-------|--------------|
|☑|`emily1@gmail.com` |`Emily`|`111-222-5556`|
|☑|`greg22@gmail.com` |`Greg` |`213-444-444` |
|☑|`john123@gmail.com`|`John` |`111-222-333` |
|☑|`ka321@gmail.com`  |`Kathy`|`111-555-330` |

</div>

---


### :red_circle: Scenario: Normalizing contact details :watch:`9ms`
> This scenario presents failures captured by VerifiableTable

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN I added contacts "`<$contacts>`" :watch:`<1ms`
**$contacts**:
<div style="overflow-x: auto;">

|Email                |Name   |PhoneNumber     |
|---------------------|-------|----------------|
|`john253@mymail.com` |`John` |`00441123344555`|
|`jenny213@mymail.com`|`Jenny`|`112334455`     |
|`jerry123@mymail.com`|`Jerry`|`1123344556`    |
|`jos#@mymail.com`    |`Josh` |`12111333444`   |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I request contacts sorted by name :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 3: THEN I should receive contacts "`<$contacts>`❗" :watch:`6ms`
**$contacts**:
<div style="overflow-x: auto;">

|#|Name   |Email                                                  |PhoneNumber                           |
|-|-------|-------------------------------------------------------|--------------------------------------|
|❗|`Jenny`|`jenny213@mymail.com`                                  |`112334455` / `matches '[0-9]{10,14}'`|
|☑|`Jerry`|`jerry123@mymail.com`                                  |`1123344556`                          |
|☑|`John` |`john253@mymail.com`                                   |`00441123344555`                      |
|❗|`Josh` |`jos#@mymail.com` / `matches '[a-z0-9.-]+@[a-z0-9.-]+'`|`12111333444`                         |

</div>

> [!IMPORTANT]
> <pre>
> Step 3: System.InvalidOperationException : Parameter 'contacts' verification failed: [0].PhoneNumber: expected: matches '[0-9]{10,14}', but got: '112334455'
> 		[3].Email: expected: matches '[a-z0-9.-]+@[a-z0-9.-]+', but got: 'jos#@mymail.com'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


### :red_circle: Scenario: Searching for contacts by phone :watch:`17ms`
> This scenario presents failures captured by VerifiableTree

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN my contact book is empty :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND I added contacts "`<$contacts>`" :watch:`<1ms`
**$contacts**:
<div style="overflow-x: auto;">

|Email              |Name   |PhoneNumber   |
|-------------------|-------|--------------|
|`john123@gmail.com`|`John` |`111-222-333` |
|`jo@hotmail.com`   |`John` |`111-303-404` |
|`greg22@gmail.com` |`Greg` |`213-444-444` |
|`emily1@gmail.com` |`Emily`|`111-222-5556`|
|`ka321@gmail.com`  |`Kathy`|`111-555-330` |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: WHEN I search for contacts by phone starting with "`111`" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 4: THEN I should receive contacts "`<$contacts>`❗" :watch:`7ms`
**$contacts**:
<div style="overflow-x: auto;">

|#|Name               |Email                                   |PhoneNumber             |
|-|-------------------|----------------------------------------|------------------------|
|☑|`Emily`            |`emily1@gmail.com`                      |`111-222-5556`          |
|❗|`John`             |`john123@gmail.com` / `john@hotmail.com`|`111-222-333`           |
|☑|`John`             |`jo@hotmail.com`                        |`111-303-404`           |
|➖|`<none>` / `Kathie`|`<none>` / `ka321@gmail.com`            |`<none>` / `111-555-330`|
|➕|`Kathy` / `<none>` |`ka321@gmail.com` / `<none>`            |`111-555-330` / `<none>`|

</div>

> [!IMPORTANT]
> <pre>
> Step 4: System.InvalidOperationException : Parameter 'contacts' verification failed: [1].Email: expected: equals 'john@hotmail.com', but got: 'john123@gmail.com'
> 		[3].Name: missing value
> 		[3].Email: missing value
> 		[3].PhoneNumber: missing value
> 		[4].Name: unexpected value
> 		[4].Email: unexpected value
> 		[4].PhoneNumber: unexpected value
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


## Customer journey :label:`Story-7`
> In order to receive a product
> As an application user
> I want to go through entire customer journey


### :warning: Scenario: Ordering products :label:`Ticket-12` :watch:`4s 350ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN customer is logged in :watch:`1s 758ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 1.1: GIVEN the user is about to login :watch:`1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 1.2: AND the user entered valid login :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 1.3: AND the user entered valid password :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 1.4: WHEN the user clicks login button :watch:`1s 747ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 1.5: THEN the login operation should be successful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :large_blue_diamond: Step 2: WHEN customer adds products to basket :watch:`1s 432ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 2.1: GIVEN product "`wooden desk`" is in stock :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :large_blue_diamond: Step 2.2: WHEN customer adds product "`wooden desk`" to the basket :watch:`1s 429ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 2.3: THEN the product addition should be successful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND customer pays for products in basket :watch:`1s 146ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 3.1: WHEN customer requests to pay :watch:`1s 145ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_check_mark: Step 3.2: THEN payment should be successful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :warning: Step 4: THEN customer should receive order email :watch:`5ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :warning: Step 4.1: THEN customer should receive invoice :watch:`3ms`
#### &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; :white_circle: Step 4.2: AND customer should receive order confirmation
> [!IMPORTANT]
> <pre>
> Step 2.2: Until proper api is implemented, product is added directly to the DB.
> Step 4.1: Not implemented yet
> </pre>

> [!NOTE]
> Step 4.1: [:link: invoice-content](ccc596a2-c0a8-4a43-abd1-62d4d05c4c84.txt)

---


## Internationalization feature :label:`Story-10`
> In order to easily use the website
> As a user
> I want to see the website in my language


### :white_check_mark: Scenario: Displaying home page in "`DE`" :watch:`3ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a customer with "`DE`" language selected :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN the customer opens the home page :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN header should display "`DE`" language :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND page title should be translated :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: AND all menu items should be translated :watch:`<1ms`
---


### :white_check_mark: Scenario: Displaying home page in "`EN`" :watch:`29ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a customer with "`EN`" language selected :watch:`2ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN the customer opens the home page :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN header should display "`EN`" language :watch:`10ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND page title should be translated :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: AND all menu items should be translated :watch:`<1ms`
---


### :white_check_mark: Scenario: Displaying home page in "`PL`" :watch:`4ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a customer with "`PL`" language selected :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN the customer opens the home page :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN header should display "`PL`" language :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND page title should be translated :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: AND all menu items should be translated :watch:`<1ms`
---


## Invoice feature :label:`Story-2`
> In order to pay for products
> As a customer
> I want to receive invoice for bought items


### :warning: Scenario: Receiving invoice for products :label:`Ticket-4` :file_folder:`Sales` :watch:`28ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN product "`wooden desk`" is available in product storage :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND product "`wooden shelf`" is available in product storage :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: WHEN customer buys product "`wooden desk`" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: AND customer buys product "`wooden shelf`" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :warning: Step 5: THEN an invoice should be sent to the customer :watch:`1ms`
#### &nbsp;&nbsp;&nbsp; :white_circle: Step 6: AND the invoice should contain product "`wooden desk`" with price of "`£62.00`"
#### &nbsp;&nbsp;&nbsp; :white_circle: Step 7: AND the invoice should contain product "`wooden shelf`" with price of "`£37.00`"
> [!IMPORTANT]
> <pre>
> Step 5: Not implemented yet
> </pre>

---


## Invoice history feature :label:`STORY-9`
> In order to see all payment details
> As a customer
> I want to browse historical invoices
> 
> Example usage of fluent scenarios


### :white_check_mark: Scenario: Browsing invoices :label:`Ticket-14` :watch:`4s 297ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN invoice "`Invoice-1`" :watch:`1s 086ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND invoice "`Invoice-2`" :watch:`1s 750ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: WHEN I request all historical invoices :watch:`1s 448ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: THEN I should see invoices "`Invoice-1, Invoice-2`" :watch:`<1ms`
---


## Login feature :label:`Story-1`
> In order to access personal data
> As an user
> I want to login into system


### :red_circle: Scenario: Anonymous login name should allow to log in :label:`Ticket-3` :file_folder:`Security` :watch:`1s 457ms`
> This scenario presents how LightBDD reports failed steps

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN the user is about to login :watch:`7ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND the user entered anonymous login :watch:`1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: WHEN the user clicks login button :watch:`1s 428ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 4: THEN the login operation should be successful :watch:`9ms`
#### &nbsp;&nbsp;&nbsp; :white_circle: Step 5: AND a welcome message containing user name should be returned
> [!IMPORTANT]
> <pre>
> Step 4: NUnit.Framework.AssertionException :   Login should succeeded
> 	  Expected: True
> 	  But was:  False
> 	
> 	at Example.LightBDD.NUnit3.Features.Login_feature.Then_the_login_operation_should_be_successful() in d:\dev\LightBDD\examples\Example.LightBDD.NUnit3\Features\Login_feature.Steps.cs:line 57
> 	at LightBDD.Framework.Scenarios.Implementation.BasicStepCompiler.StepExecutor.Execute(Object context, Object[] args) in d:\dev\LightBDD\src\LightBDD.Framework\Scenarios\Implementation\BasicStepCompiler.cs:line 102
> </pre>

> [!NOTE]
> <pre>
> Step 2: Presentation of failed scenario
> </pre>

---


### :white_check_mark: Scenario: Successful login :label:`Ticket-1` :file_folder:`Security` :watch:`1s 484ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN the user is about to login :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND the user entered valid login :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND the user entered valid password :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: WHEN the user clicks login button :watch:`1s 484ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: THEN the login operation should be successful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 6: AND a welcome message containing user name should be returned :watch:`<1ms`
---


### :white_check_mark: Scenario: Wrong login provided causes login to fail :label:`Ticket-2` :file_folder:`Security` :watch:`1s 704ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN the user is about to login :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND the user entered invalid login :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND the user entered valid password :watch:`2ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: WHEN the user clicks login button :watch:`1s 701ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: THEN the login operation should be unsuccessful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 6: AND an invalid login or password error message should be returned :watch:`<1ms`
---


### :white_check_mark: Scenario: Wrong password provided causes login to fail :label:`Ticket-2` :file_folder:`Security` :watch:`1s 945ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN the user is about to login :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND the user entered valid login :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND the user entered invalid password :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: WHEN the user clicks login button :watch:`1s 945ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: THEN the login operation should be unsuccessful :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 6: AND an invalid login or password error message should be returned :watch:`<1ms`
---


## Payment feature :label:`Story-5`
> In order to get desired products
> As a customer
> I want to pay for products in basket


### :white_check_mark: Scenario: Successful payment :label:`Ticket-10``Ticket-11` :watch:`5s 925ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN customer has some products in basket :watch:`1s 580ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND customer has enough money to pay for products :watch:`1s 400ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: WHEN customer requests to pay :watch:`1s 131ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: THEN payment should be successful :watch:`1s 802ms`
---


## Product spedition feature :label:`Story-3`
> In order to deliver products to customer effectively
> As a spedition manager
> I want to dispatch products to customer as soon as the payment is finalized


### :white_check_mark: Scenario: Should dispatch product after payment is finalized :label:`Ticket-5` :file_folder:`Delivery``Sales` :watch:`20ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN There is an active customer with id "`ABC-123`" :watch:`2ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: AND The customer has product "`wooden shelf`" in basket :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: AND The customer has product "`wooden desk`" in basket :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 4: WHEN The customer payment finalizes :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 5: THEN Product "`wooden shelf`" should be dispatched to the customer :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 6: AND Product "`wooden desk`" should be dispatched to the customer :watch:`<1ms`
---


## Record persistence feature


### :red_circle: Scenario: Saving data :watch:`2s 057ms`
> This scenario presents failures captured by VerifiableTree using ExpectEquivalent mode

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN no saved records :watch:`3ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I save records "`<$records>`" :watch:`2s 033ms`
**$records**:
<div style="overflow-x: auto;">

| Node | Value |
| ---- | ----- |
| `$` | `<array:2>` |
| `$[0]` | `Record 1` |
| `$[1]` | `Record 2` |

</div>

#### &nbsp;&nbsp;&nbsp; :red_circle: Step 3: THEN the saved records should match expectation "`<$expectation>`❗" :watch:`5ms`
**$expectation**:
<div style="overflow-x: auto;">

| # | Node | Value |
|---| ---- | ----- |
| ☑ | `$` | `<array:2>` |
| ☑ | `$[0]` | `<object>` |
| ☑ | `$[0].Id` | `1f1e2505-44ec-4868-9c1f-c43fba570118` |
| ☑ | `$[0].ModifiedDate` | `01/05/2025 20:26:20 +00:00` |
| ❗ | `$[0].Name` | `like 'Recr*'` / `Record 1` |
| ☑ | `$[1]` | `<object>` |
| ❗ | `$[1].Id` | `equals '00000000-0000-0000-0000-000000000000'` / `b490fca5-8ad2-4d61-921c-6e98b21b5dac` |
| ❗ | `$[1].ModifiedDate` | `less than '01/05/2025 20:26:20 +00:00'` / `01/05/2025 20:26:21 +00:00` |
| ☑ | `$[1].Name` | `Record 2` |

</div>

> [!IMPORTANT]
> <pre>
> Step 3: System.InvalidOperationException : Parameter 'expectation' verification failed: $[0].Name: expected: like 'Recr*', but got: 'Record 1'
> 		$[1].Id: expected: equals '00000000-0000-0000-0000-000000000000', but got: 'b490fca5-8ad2-4d61-921c-6e98b21b5dac'
> 		$[1].ModifiedDate: expected: less than '01/05/2025 20:26:20 +00:00', but got: '01/05/2025 20:26:21 +00:00'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


## User management feature :label:`Story-11`
> In order to manage users
> As an admin
> I want to be able to retrieve user data


### :red_circle: Scenario: Retrieving user details :watch:`50ms`
> This scenario presents failures captured by Verifiable<T>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a user with id "`124`" name "`Joe`" surname "`Johnson`" and email "`jj@gmail.com`" :watch:`2ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I request user details for id "`124`" :watch:`1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 3: THEN I should receive user with id "`124`☑" name "`expected: equals 'Joe', but got: 'JOE'`❗" surname "`expected: equals 'Johnson', but got: 'JOHNSON'`❗" and email "`jj@gmail.com`☑" :watch:`30ms`
> [!IMPORTANT]
> <pre>
> Step 3: System.InvalidOperationException : Parameter 'name' verification failed: expected: equals 'Joe', but got: 'JOE'
> 	Parameter 'surname' verification failed: expected: equals 'Johnson', but got: 'JOHNSON'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


### :red_circle: Scenario: User search :watch:`18ms`
> This scenario presents failures captured by VerifiableTable

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN users "`<$users>`" :watch:`<1ms`
**$users**:
<div style="overflow-x: auto;">

|Email             |Id |Name    |Surname   |
|------------------|---|--------|----------|
|`jj@foo.com`      |`1`|`Joe`   |`Andersen`|
|`henry123@foo.com`|`2`|`Henry` |`Hansen`  |
|`ma31@bar.com`    |`3`|`Marry` |`Davis`   |
|`monsmi22@bar.com`|`4`|`Monica`|`Larsen`  |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I search for users by surname pattern "`.*sen`" :watch:`5ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 3: THEN I should receive users "`<$users>`❗" :watch:`7ms`
**$users**:
<div style="overflow-x: auto;">

|#|Email                        |Id            |Name               |Surname            |
|-|-----------------------------|--------------|-------------------|-------------------|
|❗|`jj@foo.com`                 |`1`           |`Joe` / `Josh`     |`Andersen`         |
|☑|`henry123@foo.com`           |`2`           |`Henry`            |`Hansen`           |
|➕|`monsmi22@bar.com` / `<none>`|`4` / `<none>`|`Monica` / `<none>`|`Larsen` / `<none>`|

</div>

> [!IMPORTANT]
> <pre>
> Step 3: System.InvalidOperationException : Parameter 'users' verification failed: [0].Name: expected: equals 'Josh', but got: 'Joe'
> 		[2].Email: unexpected value
> 		[2].Id: unexpected value
> 		[2].Name: unexpected value
> 		[2].Surname: unexpected value
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


### :red_circle: Scenario: Validating found users :watch:`8ms`
> This scenario presents failures captured by VerifiableTable with usage of Expect.To

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN users "`<$users>`" :watch:`<1ms`
**$users**:
<div style="overflow-x: auto;">

|Email              |Id |Name    |Surname   |
|-------------------|---|--------|----------|
|`jj@foo.com`       |`0`|`Joe`   |`Andersen`|
|`henry123@foo2.com`|`2`|`Henry` |`Hansen`  |
|`ma31@bar.com`     |`3`|`Marry` |`Davis`   |
|`monsmi22@bar.com` |`4`|`Monica`|`Larsen`  |

</div>

#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I search for users by surname pattern "`.*sen`" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :red_circle: Step 3: THEN I should receive users "`<$users>`❗" :watch:`4ms`
**$users**:
<div style="overflow-x: auto;">

|#|Id                      |Name    |Surname   |Email                                                                  |
|-|------------------------|--------|----------|-----------------------------------------------------------------------|
|❗|`0` / `greater than '0'`|`Joe`   |`Andersen`|`jj@foo.com`                                                           |
|❗|`2`                     |`Henry` |`Hansen`  |`henry123@foo2.com` / `matches '[\w]+@([a-z]+)(\.[a-z]+)+' ignore case`|
|☑|`4`                     |`Monica`|`Larsen`  |`monsmi22@bar.com`                                                     |

</div>

> [!IMPORTANT]
> <pre>
> Step 3: System.InvalidOperationException : Parameter 'users' verification failed: [0].Id: expected: greater than '0', but got: '0'
> 		[1].Email: expected: matches '[\w]+@([a-z]+)(\.[a-z]+)+' ignore case, but got: 'henry123@foo2.com'
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.VerifyParameterResults() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 174
> 	at LightBDD.Core.Execution.Implementation.RunnableStep.RunStepAsync() in d:\dev\LightBDD\src\LightBDD.Core\Execution\Implementation\RunnableStep.cs:line 146
> </pre>

---


### :white_check_mark: Scenario: Validating user details :watch:`10ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 1: GIVEN a user with id "`124`" name "`Joe`" surname "`Johnson`" and email "`jj@gmail.com`" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 2: WHEN I request user details for id "`124`" :watch:`<1ms`
#### &nbsp;&nbsp;&nbsp; :white_check_mark: Step 3: THEN I should receive user with id "`124`☑" name "`JOE`☑" surname "`JOHNSON`☑" and email "`jj@gmail.com`☑" :watch:`<1ms`
---
