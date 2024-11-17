package helper

import "fmt"

func Action(a int, b int) int {
	return a * b
}

// Hi returns a friendly greeting
func Hi(name string) string {
	return fmt.Sprintf("Hi, %s", name)
}
