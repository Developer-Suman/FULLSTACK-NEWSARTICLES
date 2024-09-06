import { Flex, Input, InputGroup, InputRightElement } from "@chakra-ui/react"


export const RightContent = ()=>{
    return(
        <Flex alignItems="center" gap={2}>
            <InputGroup size="sm" display={{base:"none", md:"flex"}}>
            <Input variant="filled" placeholder="Search..."/>
            <InputRightElement>
            <FaSearch color="teal"/>
            </InputRightElement>
            </InputGroup>

        </Flex>
    )

}