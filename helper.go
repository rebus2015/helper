package helper

import "fmt"

func Action(a int, b int, c int) int {
	return a * b * c
}

// Hi returns a friendly greeting
func Hi(name string) string {
	return fmt.Sprintf("Hi, %s!", name)
}
